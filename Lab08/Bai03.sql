create database Net107_Lab8_Bai03
go
use Net107_Lab8_Bai03
go
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50),
    Password NVARCHAR(50)
);
-- Thêm dữ liệu mẫu
INSERT INTO Users (Username, Password) VALUES ('admin', '123');
INSERT INTO Users (Username, Password) VALUES ('teopy', '123');