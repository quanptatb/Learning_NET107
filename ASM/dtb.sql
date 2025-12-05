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
    PasswordHash varchar(255) NOT NULL,      -- SHA-256 hash in production
    Role nvarchar(20) DEFAULT N'Staff',
    CreatedDate date DEFAULT GETDATE(),
    IsActive bit DEFAULT 1,
    CONSTRAINT UK_Employees_Email UNIQUE(Email),
    CONSTRAINT UK_Employees_Username UNIQUE(Username),
    CONSTRAINT CK_Employees_Phone CHECK (Phone LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' 
                                        AND LEN(Phone) IN (10,11))
);

--------------------------------------------------------------------
-- 2. Products
--------------------------------------------------------------------
CREATE TABLE Products (
    ProductID varchar(10) PRIMARY KEY,
    ProductName nvarchar(100) NOT NULL,
    UnitPrice decimal(18,2) NOT NULL CHECK (UnitPrice >= 0),
    StockQuantity int NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0)
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
    Status bit DEFAULT 0,               -- 0 = Pending, 1 = Paid
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
-- TRIGGERS: PURCHASE → STOCK IN
--------------------------------------------------------------------
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

--------------------------------------------------------------------
-- TRIGGERS: SALES → STOCK OUT
--------------------------------------------------------------------
-- Insert detail (only subtract stock if invoice is already Paid)
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

-- Update detail
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

-- Delete detail
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

-- When invoice status changes from Pending → Paid
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

-- Auto calculate TotalAmount
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

-- Prevent changing ProductID in detail
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
-- SAMPLE DATA (FULL ENGLISH)
--------------------------------------------------------------------
-- Employees
INSERT INTO Employees (EmployeeID, FullName, Email, Phone, Username, PasswordHash, Role, IsActive) VALUES
('NV001', N'Nguyễn Văn Admin',   'admin@shop.com',   '0901234567', 'admin',     '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Manager', 1),
('NV002', N'Trần Thị Hương',    'huong@shop.com',   '0912345678', 'staff1',    'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Staff',   1),
('NV003', N'Lê Văn Nam',        'nam@shop.com',     '0923456789', 'staff2',    'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Staff',   1);
-- password of staff1 & staff2 = 123456 (SHA-256)

-- Suppliers
INSERT INTO Suppliers (SupplierID, SupplierName, Email, Phone) VALUES
('SUP01', N'VinEco Clean Food Co., Ltd',          'vineco@gmail.com',         '0988111222'),
('SUP02', N'Vinamilk Vietnam Joint Stock Company','vinamilk@vinamilk.com.vn','0285415522'),
('SUP03', N'Saigon Beer Alcohol Beverage Corp',   'sabeco@sabeco.com.vn',     '0283829526'),
('SUP04', N'Acecook Vietnam Joint Stock Company', 'acecook@acecook.vn',       '02838984567'),
('SUP05', N'Suntory PepsiCo Vietnam Beverage',    'pepsi@pepsi.vn',           '02839104999');

-- Products (20 items)
INSERT INTO Products (ProductID, ProductName, UnitPrice, StockQuantity) VALUES
('SP001', N'Vinamilk Fresh Milk No Sugar 1L',      32000, 150),
('SP002', N'Vinamilk Yogurt 100g',                 6000,  300),
('SP003', N'Saigon Export Beer Can 330ml',          14500, 500),
('SP004', N'Heineken Silver Can 330ml',             22500, 400),
('SP005', N'Hao Hao Instant Noodles Sour Shrimp',   4500,  1000),
('SP006', N'Omachi Instant Noodles Beef',           5500,  800),
('SP007', N'Pepsi Cola Can 330ml',                 10000, 600),
('SP008', N'Coca Cola Can 330ml',                  11000, 550),
('SP009', N'Aquafina Mineral Water 500ml',          6000,  700),
('SP010', N'C2 Green Tea 455ml',                    8500,  400),
('SP011', N'Oreo Chocolate Cookies 120g',           18500, 250),
('SP012', N'Snickers Chocolate Bar 51g',            14000, 300),
('SP013', N'ST25 Rice 5kg Bag',                    145000,80),
('SP014', N'Simply Soybean Cooking Oil 5L',        185000,60),
('SP015', N'Bien Hoa White Sugar 1kg',              24000, 200),
('SP016', N'Phu Quoc Fish Sauce 520ml',             42000, 150),
('SP017', N'Vifon Iodized Salt 1kg',                8500,  300),
('SP018', N'Ajinomoto MSG 1kg',                     68000, 100),
('SP019', N'Chinsu Chili Sauce 250g',               18000, 400),
('SP020', N'Safoco Dried Noodles 400g',             16000, 250);

-- Customers
INSERT INTO Customers (CustomerID, CustomerName, Phone) VALUES
('KH001', N'Nguyễn Thị Lan',      '0905123456'),
('KH002', N'Trần Văn Hùng',       '0918234567'),
('KH003', N'Phạm Minh Tuấn',      '0934567890'),
('KH004', N'Lê Thị Mai',          '0945678901'),
('KH005', N'Vũ Văn Hoàng',        '0956789012'),
('KH006', N'Hoàng Thị Ngọc',      '0967890123'),
('KH007', N'Đỗ Văn Khánh',        '0978901234'),
('KH008', N'Bùi Thị Hà',          '0989012345'),
('KH009', N'Walk-in Customer 001','0391234567'),
('KH010', N'Walk-in Customer 002','0392345678');

-- Purchase Invoices (2 mẫu)
INSERT INTO PurchaseInvoices (PurchaseID, EmployeeID, SupplierID, PurchaseDate) VALUES
('PI001', 'NV001', 'SUP02', '2025-10-15'),
('PI002', 'NV002', 'SUP03', '2025-11-01');

INSERT INTO PurchaseInvoiceDetails (PurchaseDetailID, PurchaseID, ProductID, Quantity, UnitPrice) VALUES
('PD001', 'PI001', 'SP001', 100, 28000),
('PD002', 'PI001', 'SP002', 200, 4500),
('PD003', 'PI002', 'SP003', 300, 12000),
('PD004', 'PI002', 'SP004', 200, 19000);

-- Sales Invoices + Details
-- Invoice 001 - Paid
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI001', 'KH001', 'NV002', 1, '2025-11-10');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD001', 'SI001', 'SP001', 32000, 5),
('SD002', 'SI001', 'SP003', 14500, 12),
('SD003', 'SI001', 'SP007', 10000, 6);

-- Invoice 002 - Paid
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI002', 'KH003', 'NV003', 1, '2025-11-12');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD004', 'SI002', 'SP005', 4500,  20),
('SD005', 'SI002', 'SP008', 11000, 10),
('SD006', 'SI002', 'SP011', 18500, 5);

-- Invoice 003 - Pending
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI003', 'KH005', 'NV002', 0, '2025-11-18');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD007', 'SI003', 'SP004', 22500, 10),
('SD008', 'SI003', 'SP009', 6000,  24);

-- Invoice 004 - Paid
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI004', 'KH002', 'NV001', 1, '2025-11-17');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD009', 'SI004', 'SP013', 145000, 2),
('SD010', 'SI004', 'SP014', 185000, 1);

-- Invoice 005 - Pending
INSERT INTO SalesInvoices (InvoiceID, CustomerID, EmployeeID, Status, InvoiceDate) VALUES ('SI005', 'KH008', 'NV003', 0, '2025-11-18');
INSERT INTO SalesInvoiceDetails (DetailID, InvoiceID, ProductID, UnitPrice, Quantity) VALUES
('SD011', 'SI005', 'SP006', 5500, 30),
('SD012', 'SI005', 'SP019', 18000, 8);

--------------------------------------------------------------------
-- Check results
--------------------------------------------------------------------
SELECT InvoiceID, TotalAmount FROM SalesInvoices ORDER BY InvoiceID;
SELECT ProductID, ProductName, StockQuantity FROM Products ORDER BY ProductID;

-- Test payment trigger (uncomment to test)
-- UPDATE SalesInvoices SET Status = 1 WHERE InvoiceID = 'SI003'