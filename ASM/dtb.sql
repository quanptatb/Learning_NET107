USE master
GO
IF DB_ID('ASM_Net107') IS NOT NULL DROP DATABASE ASM_Net107
GO
CREATE DATABASE ASM_Net107
GO
USE ASM_Net107
GO

--------------------------------------------------------------------
-- 1. Employees
--------------------------------------------------------------------
CREATE TABLE Employees (
    EmployeeID varchar(10) PRIMARY KEY,
    FullName nvarchar(50) NOT NULL,
    Email varchar(100) NOT NULL,
    Phone varchar(15),
    Username varchar(20) NOT NULL,
    PasswordHash varchar(255) NOT NULL,
    Role nvarchar(20) DEFAULT N'Staff',
    CreatedDate date DEFAULT GETDATE(),
    IsActive bit DEFAULT 1,
    CONSTRAINT UK_Employees_Email UNIQUE(Email),
    CONSTRAINT UK_Employees_Username UNIQUE(Username),
    CONSTRAINT CK_Employees_Phone CHECK (Phone LIKE '[0-9]%' AND LEN(Phone) IN (10,11))
);

--------------------------------------------------------------------
-- 2. Products
--------------------------------------------------------------------
CREATE TABLE Products (
    ProductID varchar(10) PRIMARY KEY,
    ProductName nvarchar(100) NOT NULL,
    UnitPrice decimal(18,2) NOT NULL CHECK (UnitPrice >= 0),
    StockQuantity int NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    ImageURL nvarchar(255) NULL -- Cột hình ảnh mới
);

--------------------------------------------------------------------
-- 3. Customers
--------------------------------------------------------------------
CREATE TABLE Customers (
    CustomerID varchar(10) PRIMARY KEY,
    CustomerName nvarchar(50) NOT NULL,
    Phone varchar(15) NOT NULL,
    CreatedDate date DEFAULT GETDATE(),
    CONSTRAINT UK_Customers_Phone UNIQUE(Phone)
);

--------------------------------------------------------------------
-- 4. Sales Invoices (Orders)
--------------------------------------------------------------------
CREATE TABLE SalesInvoices (
    InvoiceID varchar(10) PRIMARY KEY,
    CustomerID varchar(10) NOT NULL,
    EmployeeID varchar(10) NOT NULL,
    Status bit DEFAULT 0,                -- 0 = Pending, 1 = Paid
    InvoiceDate date DEFAULT GETDATE(),
    TotalAmount decimal(18,2) NULL,
    CONSTRAINT FK_SalesInvoices_Customers FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    CONSTRAINT FK_SalesInvoices_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

--------------------------------------------------------------------
-- 5. Sales Invoice Details
--------------------------------------------------------------------
CREATE TABLE SalesInvoiceDetails (
    DetailID varchar(10) PRIMARY KEY,
    InvoiceID varchar(10) NOT NULL,
    ProductID varchar(10) NOT NULL,
    UnitPrice decimal(18,2) NOT NULL,
    Quantity int NOT NULL CHECK (Quantity > 0),
    CONSTRAINT FK_SalesDetails_Invoice FOREIGN KEY (InvoiceID) REFERENCES SalesInvoices(InvoiceID) ON DELETE CASCADE,
    CONSTRAINT FK_SalesDetails_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

--------------------------------------------------------------------
-- 6. Suppliers
--------------------------------------------------------------------
CREATE TABLE Suppliers (
    SupplierID varchar(10) PRIMARY KEY,
    SupplierName nvarchar(100) NOT NULL,
    Email varchar(100),
    Phone varchar(15),
    CreatedDate date DEFAULT GETDATE(),
    IsActive bit DEFAULT 1,
    CONSTRAINT UK_Suppliers_Email UNIQUE(Email)
);

--------------------------------------------------------------------
-- 7. Purchase Invoices
--------------------------------------------------------------------
CREATE TABLE PurchaseInvoices (
    PurchaseID varchar(10) PRIMARY KEY,
    EmployeeID varchar(10) NOT NULL,
    SupplierID varchar(10) NOT NULL,
    PurchaseDate date DEFAULT GETDATE(),
    CONSTRAINT FK_Purchase_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    CONSTRAINT FK_Purchase_Suppliers FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

--------------------------------------------------------------------
-- 8. Purchase Invoice Details
--------------------------------------------------------------------
CREATE TABLE PurchaseInvoiceDetails (
    PurchaseDetailID varchar(10) PRIMARY KEY,
    PurchaseID varchar(10) NOT NULL,
    ProductID varchar(10) NOT NULL,
    Quantity int NOT NULL CHECK (Quantity > 0),
    UnitPrice decimal(18,2) NOT NULL CHECK (UnitPrice >= 0),
    CONSTRAINT FK_PurchaseDetails_Purchase FOREIGN KEY (PurchaseID) REFERENCES PurchaseInvoices(PurchaseID) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseDetails_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

--------------------------------------------------------------------
-- Indexes
--------------------------------------------------------------------
CREATE INDEX IX_Products_ProductName ON Products(ProductName);
CREATE INDEX IX_Customers_Phone ON Customers(Phone);
CREATE INDEX IX_SalesInvoices_InvoiceDate ON SalesInvoices(InvoiceDate);
GO

--------------------------------------------------------------------
-- TRIGGERS (GIỮ NGUYÊN NHƯ CŨ)
--------------------------------------------------------------------
-- Trigger: PURCHASE → STOCK IN
CREATE OR ALTER TRIGGER trg_Purchase_Insert
ON PurchaseInvoiceDetails AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    MERGE Products AS t
    USING (SELECT ProductID, SUM(Quantity) AS Qty FROM inserted GROUP BY ProductID) AS i
        ON t.ProductID = i.ProductID
    WHEN MATCHED THEN UPDATE SET StockQuantity = StockQuantity + i.Qty
    WHEN NOT MATCHED THEN INSERT (ProductID, StockQuantity) VALUES (i.ProductID, i.Qty);
END
GO

CREATE OR ALTER TRIGGER trg_Purchase_Update
ON PurchaseInvoiceDetails AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF NOT (UPDATE(Quantity) OR UPDATE(ProductID)) RETURN;

    MERGE Products AS t USING (SELECT ProductID, SUM(Quantity) AS Qty FROM deleted GROUP BY ProductID) AS d
        ON t.ProductID = d.ProductID
    WHEN MATCHED THEN UPDATE SET StockQuantity = StockQuantity - d.Qty;

    MERGE Products AS t USING (SELECT ProductID, SUM(Quantity) AS Qty FROM inserted GROUP BY ProductID) AS i
        ON t.ProductID = i.ProductID
    WHEN MATCHED THEN UPDATE SET StockQuantity = StockQuantity + i.Qty
    WHEN NOT MATCHED THEN INSERT (ProductID, StockQuantity) VALUES (i.ProductID, i.Qty);
END
GO

CREATE OR ALTER TRIGGER trg_Purchase_Delete
ON PurchaseInvoiceDetails AFTER DELETE AS
BEGIN
    SET NOCOUNT ON;
    MERGE Products AS t USING (SELECT ProductID, SUM(Quantity) AS Qty FROM deleted GROUP BY ProductID) AS d
        ON t.ProductID = d.ProductID
    WHEN MATCHED THEN UPDATE SET StockQuantity = StockQuantity - d.Qty;
END
GO

-- Trigger: SALES → STOCK OUT
CREATE OR ALTER TRIGGER trg_SalesDetail_Insert
ON SalesInvoiceDetails AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM SalesInvoices si JOIN inserted i ON si.InvoiceID = i.InvoiceID WHERE si.Status = 0)
        RETURN;

    IF EXISTS (SELECT 1 FROM inserted i JOIN Products p ON i.ProductID = p.ProductID
               GROUP BY p.ProductID, p.StockQuantity HAVING p.StockQuantity < SUM(i.Quantity))
    BEGIN
        RAISERROR(N'Not enough stock for one or more products!', 16, 1);
        ROLLBACK; RETURN;
    END

    UPDATE p SET StockQuantity = StockQuantity - i.TotalQty
    FROM Products p
    JOIN (SELECT ProductID, SUM(Quantity) AS TotalQty FROM inserted GROUP BY ProductID) i
        ON p.ProductID = i.ProductID;
END
GO

CREATE OR ALTER TRIGGER trg_SalesDetail_Update
ON SalesInvoiceDetails AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF NOT (UPDATE(Quantity) OR UPDATE(ProductID)) RETURN;

    ;WITH Affected AS (SELECT DISTINCT InvoiceID FROM inserted UNION SELECT DISTINCT InvoiceID FROM deleted),
          Paid AS (SELECT a.InvoiceID FROM Affected a JOIN SalesInvoices si ON si.InvoiceID = a.InvoiceID WHERE si.Status = 1)

    -- Return old quantity
    UPDATE p SET StockQuantity = StockQuantity + d.Qty
    FROM Products p
    JOIN (SELECT ProductID, SUM(Quantity) AS Qty FROM deleted d
          WHERE EXISTS (SELECT 1 FROM Paid pi WHERE pi.InvoiceID = d.InvoiceID)
          GROUP BY ProductID) d ON p.ProductID = d.ProductID;

    -- Check stock for new quantity
    IF EXISTS (SELECT 1 FROM inserted i JOIN Products p ON i.ProductID = p.ProductID
               WHERE EXISTS (SELECT 1 FROM Paid pi WHERE pi.InvoiceID = i.InvoiceID)
               GROUP BY i.ProductID, p.StockQuantity HAVING p.StockQuantity < SUM(i.Quantity))
    BEGIN
        RAISERROR(N'Not enough stock after update!', 16, 1);
        ROLLBACK; RETURN;
    END

    -- Subtract new quantity
    UPDATE p SET StockQuantity = StockQuantity - i.Qty
    FROM Products p
    JOIN (SELECT ProductID, SUM(Quantity) AS Qty FROM inserted i
          WHERE EXISTS (SELECT 1 FROM Paid pi WHERE pi.InvoiceID = i.InvoiceID)
          GROUP BY ProductID) i ON p.ProductID = i.ProductID;
END
GO

CREATE OR ALTER TRIGGER trg_SalesDetail_Delete
ON SalesInvoiceDetails AFTER DELETE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p SET StockQuantity = StockQuantity + d.Qty
    FROM Products p
    JOIN (SELECT ProductID, SUM(Quantity) AS Qty FROM deleted d
          JOIN SalesInvoices si ON d.InvoiceID = si.InvoiceID AND si.Status = 1
          GROUP BY ProductID) d ON p.ProductID = d.ProductID;
END
GO

CREATE OR ALTER TRIGGER trg_PayInvoice
ON SalesInvoices AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(Status) RETURN;

    DECLARE @InvoiceID varchar(10);
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT i.InvoiceID FROM inserted i
        JOIN deleted d ON i.InvoiceID = d.InvoiceID
        WHERE d.Status = 0 AND i.Status = 1;

    OPEN cur;
    FETCH NEXT FROM cur INTO @InvoiceID;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM SalesInvoiceDetails d JOIN Products p ON d.ProductID = p.ProductID
                   WHERE d.InvoiceID = @InvoiceID
                   GROUP BY p.ProductID, p.StockQuantity HAVING p.StockQuantity < SUM(d.Quantity))
        BEGIN
            RAISERROR(N'Invoice %s: Not enough stock to complete payment!', 16, 1, @InvoiceID);
            ROLLBACK; CLOSE cur; DEALLOCATE cur; RETURN;
        END

        UPDATE p SET StockQuantity = StockQuantity - d.Qty
        FROM Products p
        JOIN (SELECT ProductID, SUM(Quantity) AS Qty FROM SalesInvoiceDetails WHERE InvoiceID = @InvoiceID GROUP BY ProductID) d
            ON p.ProductID = d.ProductID;

        FETCH NEXT FROM cur INTO @InvoiceID;
    END
    CLOSE cur; DEALLOCATE cur;
END
GO

CREATE OR ALTER TRIGGER trg_UpdateTotalAmount
ON SalesInvoiceDetails AFTER INSERT, UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE si SET TotalAmount = ISNULL((
        SELECT SUM(Quantity * UnitPrice)
        FROM SalesInvoiceDetails d WHERE d.InvoiceID = si.InvoiceID), 0)
    FROM SalesInvoices si
    WHERE si.InvoiceID IN (SELECT InvoiceID FROM inserted UNION SELECT InvoiceID FROM deleted);
END
GO

CREATE OR ALTER TRIGGER trg_PreventChangeProductID
ON SalesInvoiceDetails FOR UPDATE AS
BEGIN
    IF UPDATE(ProductID)
    BEGIN
        RAISERROR(N'Cannot change ProductID in sales detail. Delete and insert a new line instead!', 16, 1);
        ROLLBACK;
    END
END
GO

--------------------------------------------------------------------
-- SAMPLE DATA (Cập nhật sang Điện thoại & Laptop)
--------------------------------------------------------------------
-- Employees
INSERT INTO Employees (EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, IsActive) VALUES
('NV001', N'Nguyễn Văn Admin',    'admin@techshop.com',   '0901234567', 'admin',     '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Manager', 1),
('NV002', N'Trần Thị Sale',     'sale1@techshop.com',    '0912345678', 'staff1',    'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Staff',   1),
('NV003', N'Lê Văn Kho',        'kho@techshop.com',      '0923456789', 'staff2',    'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Staff',   1);

-- Suppliers (Các nhà cung cấp công nghệ)
INSERT INTO Suppliers (SupplierID, SupplierName, Email, Phone) VALUES
('SUP01', N'Apple Vietnam LLC',             'contact@apple.com.vn',     '18001127'),
('SUP02', N'Samsung Vina Electronics',      'b2b@samsung.com',          '02839157600'),
('SUP03', N'FPT Synnex',                    'info@synnexfpt.com',       '02473006666'),
('SUP04', N'Digiworld Corp',                'sales@dgw.com.vn',         '02839290059'),
('SUP05', N'PetroSetco Distribution',       'contact@petrosetco.vn',    '02839117777');

-- Products (Điện thoại & Laptop - Có ImageURL)
INSERT INTO Products (ProductID, ProductName, UnitPrice, StockQuantity, ImageURL) VALUES
-- Phones
('DT001', N'iPhone 15 Pro Max 256GB',       29500000, 50,  'iphone15promax.jpg'),
('DT002', N'iPhone 15 Plus 128GB',          22000000, 30,  'iphone15plus.jpg'),
('DT003', N'Samsung Galaxy S24 Ultra 512GB',28990000, 40,  's24ultra.jpg'),
('DT004', N'Samsung Galaxy Z Fold5',        35000000, 20,  'zfold5.jpg'),
('DT005', N'Xiaomi 14 Ultra',               24990000, 25,  'xiaomi14ultra.jpg'),
('DT006', N'OPPO Find N3 Flip',             19990000, 30,  'oppon3flip.jpg'),
('DT007', N'Google Pixel 8 Pro',            20500000, 15,  'pixel8pro.jpg'),
('DT008', N'Sony Xperia 1 V',               27990000, 10,  'xperia1v.jpg'),
('DT009', N'Asus ROG Phone 8 Pro',          26000000, 15,  'rogphone8.jpg'),
('DT010', N'iPhone 13 128GB',               13500000, 60,  'iphone13.jpg'),

-- Laptops
('LT001', N'MacBook Air M2 13 inch 8GB',    24500000, 20,  'macbookairm2.jpg'),
('LT002', N'MacBook Pro 14 M3 16GB',        45000000, 15,  'macbookprom3.jpg'),
('LT003', N'Dell XPS 13 Plus 9320',         38000000, 10,  'dellxps13.jpg'),
('LT004', N'HP Spectre x360 14',            35000000, 12,  'hpspectre.jpg'),
('LT005', N'Lenovo ThinkPad X1 Carbon Gen 11',42000000, 8, 'thinkpadx1.jpg'),
('LT006', N'Asus Zenbook 14 OLED',          21000000, 25,  'zenbook14.jpg'),
('LT007', N'Acer Predator Helios Neo 16',   32000000, 15,  'acerpredator.jpg'),
('LT008', N'MSI Gaming Katana 15',          25000000, 20,  'msikatana.jpg'),
('LT009', N'LG Gram 2023 16 inch',          29000000, 10,  'lggram16.jpg'),
('LT010', N'Surface Laptop 5 13.5',         26000000, 12,  'surface5.jpg');

-- Customers
INSERT INTO Customers (CustomerID, CustomerName, Phone) VALUES
('KH001', N'Nguyễn Thị Lan',    '0905123456'),
('KH002', N'Trần Văn Hùng',     '0918234567'),
('KH003', N'Phạm Minh Tuấn',    '0934567890'),
('KH004', N'Lê Thị Mai',        '0945678901'),
('KH005', N'Vũ Văn Hoàng',      '0956789012'),
('KH006', N'Hoàng Thị Ngọc',    '0967890123'),
('KH007', N'Đỗ Văn Khánh',      '0978901234'),
('KH008', N'Bùi Thị Hà',        '0989012345'),
('KH009', N'Khách Vãng Lai 1',  '0391234567'),
('KH010', N'Khách Vãng Lai 2',  '0392345678');

-- Purchase Invoices (Nhập hàng)
INSERT INTO PurchaseInvoices (PurchaseID, EmployeeID, SupplierID, PurchaseDate) VALUES
('PI001', 'NV001', 'SUP01', '2025-10-15'), -- Nhập từ Apple
('PI002', 'NV002', 'SUP02', '2025-11-01'); -- Nhập từ Samsung

INSERT INTO PurchaseInvoiceDetails (PurchaseDetailID, PurchaseID, ProductID, Quantity, UnitPrice) VALUES
('PD001', 'PI001', 'DT001', 20, 26000000), -- iPhone 15 Pro Max
('PD002', 'PI001', 'LT001', 10, 21000000), -- MacBook Air
('PD003', 'PI002', 'DT003', 15, 25000000), -- S24 Ultra
('PD004', 'PI002', 'DT004', 10, 31000000); -- Z Fold5

-- Sales Invoices + Details
-- Invoice 001 - Paid (Bán được hàng)
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI001', 'KH001', 'NV002', 1, '2025-11-10');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD001', 'SI001', 'DT001', 29500000, 1), -- Mua 1 iPhone
('SD002', 'SI001', 'LT006', 21000000, 1); -- Mua 1 Laptop Asus

-- Invoice 002 - Paid
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI002', 'KH003', 'NV003', 1, '2025-11-12');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD003', 'SI002', 'DT010', 13500000, 2), -- Mua 2 iPhone 13
('SD004', 'SI002', 'LT008', 25000000, 1); -- Mua 1 MSI Gaming

-- Invoice 003 - Pending (Chưa thanh toán)
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI003', 'KH005', 'NV002', 0, '2025-11-18');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD005', 'SI003', 'LT002', 45000000, 1), -- MacBook Pro
('SD006', 'SI003', 'DT009', 26000000, 1); -- ROG Phone

-- Invoice 004 - Paid
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI004', 'KH002', 'NV001', 1, '2025-11-17');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD007', 'SI004', 'LT005', 42000000, 1), -- ThinkPad X1
('SD008', 'SI004', 'DT003', 28990000, 1); -- S24 Ultra

-- Invoice 005 - Pending
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI005', 'KH008', 'NV003', 0, '2025-11-18');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD009', 'SI005', 'DT005', 24990000, 1), -- Xiaomi 14
('SD010', 'SI005', 'LT010', 26000000, 1); -- Surface Laptop

--------------------------------------------------------------------
-- Check results
--------------------------------------------------------------------
SELECT InvoiceID, TotalAmount, Status FROM SalesInvoices ORDER BY InvoiceID;
SELECT ProductID, ProductName, StockQuantity, ImageURL FROM Products ORDER BY ProductID;