CREATE DATABASE Lab7_CSharp3;
GO
USE Lab7_CSharp3;
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Category NVARCHAR(50),
    Color NVARCHAR(50),
    UnitPrice DECIMAL(18, 2),
    AvailableQuantity INT,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Thêm một vài dữ liệu mẫu
INSERT INTO Products (Name, Category, Color, UnitPrice, AvailableQuantity, CreatedDate)
VALUES 
('Fpoly hcm', 'Cntt', 'Blue', 20.00, 1, '2020-05-17 12:23:48'),
('Camera Leica', 'Camera', 'Black', 15995.00, 5, GETDATE());