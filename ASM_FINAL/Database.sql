CREATE DATABASE ASM;
GO
USE ASM;
GO

-- 1. Bảng Categories (Danh mục)
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

-- 2. Bảng Users (Dùng chung cho Admin, Staff, Customer)
-- Role: 0=Admin, 1=Staff, 2=Customer
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(50) NOT NULL, -- Trong thực tế nên mã hóa
    FullName NVARCHAR(100),
    Email VARCHAR(100),
    Role INT DEFAULT 2
);

-- 3. Bảng Products (Sản phẩm)
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(18,0) NOT NULL,
    Image NVARCHAR(MAX),
    Color NVARCHAR(50),
    Size NVARCHAR(20),
    Description NVARCHAR(MAX),
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id)
);

-- 4. Bảng Carts (Giỏ hàng)
CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- 5. Bảng Cart_Detail (Chi tiết giỏ hàng)
CREATE TABLE Cart_Detail (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CartId INT FOREIGN KEY REFERENCES Carts(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    Quantity INT DEFAULT 1,
    Price DECIMAL(18,0)
);

-- NHẬP DỮ LIỆU MẪU (SEED DATA)
-- 2 Danh mục
INSERT INTO Categories (Name) VALUES (N'Điện thoại'), (N'Laptop');

-- 3 Người dùng (Admin, Staff, Customer)
INSERT INTO Users (Username, Password, FullName, Role) VALUES 
('admin', '123', N'Quản Lý', 0),
('staff', '123', N'Nhân Viên', 1),
('customer', '123', N'Khách Hàng A', 2);

-- 15 Sản phẩm
INSERT INTO Products (Name, Price, Image, Color, Size, CategoryId) VALUES 
(N'iPhone 15', 20000000, 'iphone15.jpg', N'Titan', 'Pro', 1),
(N'Samsung S24', 18000000, 's24.jpg', N'Đen', 'Standard', 1),
(N'MacBook Air', 25000000, 'macbook.jpg', N'Bạc', '13 inch', 2),
(N'Dell XPS', 30000000, 'dell.jpg', N'Xám', '15 inch', 2),
(N'Xiaomi 14', 15000000, 'xiaomi.jpg', N'Xanh', 'Standard', 1),
(N'Asus Zenbook', 22000000, 'asus.jpg', N'Xanh biển', '14 inch', 2),
(N'iPhone 14', 17000000, 'iphone14.jpg', N'Trắng', 'Standard', 1),
(N'HP Spectre', 28000000, 'hp.jpg', N'Đen', '13 inch', 2),
(N'iPad Pro', 20000000, 'ipad.jpg', N'Xám', '11 inch', 2),
(N'Sony Xperia', 19000000, 'sony.jpg', N'Tím', 'Standard', 1),
(N'Lenovo ThinkPad', 24000000, 'lenovo.jpg', N'Đen', '14 inch', 2),
(N'Oppo Find X', 16000000, 'oppo.jpg', N'Cam', 'Standard', 1),
(N'Acer Swift', 18000000, 'acer.jpg', N'Bạc', '14 inch', 2),
(N'Google Pixel', 14000000, 'pixel.jpg', N'Trắng', 'Standard', 1),
(N'LG Gram', 26000000, 'lg.jpg', N'Trắng', '16 inch', 2);

-- Cập nhật bảng Carts để quản lý trạng thái đơn hàng
ALTER TABLE Carts ADD Status INT DEFAULT 0; 
-- 0: Đang mua (Giỏ hàng), 1: Chờ duyệt, 2: Đã duyệt, 3: Đã hủy