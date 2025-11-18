USE master
GO
CREATE DATABASE ASM_Net107
GO
USE ASM_Net107
GO

-- 1. Nhân viên
CREATE TABLE NhanVien (
    MaNV varchar(10) PRIMARY KEY,
    TenNV nvarchar(50) NOT NULL,
    Email varchar(100) NOT NULL,
    SDT varchar(15),
    TenDangNhap varchar(20) NOT NULL,
    MatKhau varchar(255) NOT NULL,  -- phải hash khi dùng thật
    ChucVu nvarchar(20) DEFAULT N'Nhân viên',
    NgayTaoTK date DEFAULT GETDATE(),
    TrangThai bit DEFAULT 1,
    CONSTRAINT UK_NhanVien_Email UNIQUE(Email),
    CONSTRAINT UK_NhanVien_TenDangNhap UNIQUE(TenDangNhap),
    CONSTRAINT CK_NhanVien_SDT CHECK (SDT LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' AND LEN(SDT) IN (10,11))
);

-- 2. Sản phẩm
CREATE TABLE SanPham (
    MaSP varchar(10) PRIMARY KEY,
    TenSP nvarchar(100) NOT NULL,
    DonGia decimal(18,2) NOT NULL CHECK (DonGia >= 0),
    SoLuongTon int NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0)
);

-- 3. Khách hàng
CREATE TABLE KhachHang (
    MaKH varchar(10) PRIMARY KEY,
    TenKH nvarchar(50) NOT NULL,
    SDT varchar(15) NOT NULL,
    NgayTaoTK date DEFAULT GETDATE(),
    CONSTRAINT UK_KhachHang_SDT UNIQUE(SDT)
);

-- 4. Phiếu bán hàng
CREATE TABLE PhieuBanHang (
    MaPBH varchar(10) PRIMARY KEY,
    MaKH varchar(10) NOT NULL,
    MaNV varchar(10) NOT NULL,
    TrangThai bit default 0,           -- 1: đã thanh toán, 0: đang xử lý/hủy
    NgayTao date DEFAULT GETDATE(),
    TongTien decimal(18,2) NULL,       -- có thể tính bằng trigger hoặc view
    CONSTRAINT FK_PhieuBanHang_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    CONSTRAINT FK_PhieuBanHang_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

-- 5. Chi tiết phiếu bán
CREATE TABLE ChiTietPhieuBanHang (
    MaCTPBH varchar(10) PRIMARY KEY,
    MaPBH varchar(10) NOT NULL,
    MaSP varchar(10) NOT NULL,
    DonGia decimal(18,2) NOT NULL,
    SoLuongBan int NOT NULL CHECK (SoLuongBan > 0),
    CONSTRAINT FK_ChiTietPBH_Phieu FOREIGN KEY (MaPBH) REFERENCES PhieuBanHang(MaPBH) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietPBH_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- 6. Nhà cung cấp
CREATE TABLE NhaCungCap (
    MaNCC varchar(10) PRIMARY KEY,
    TenNCC nvarchar(100) NOT NULL,
    Email varchar(100),
    SDT varchar(15),
    NgayTaoTK date DEFAULT GETDATE(),
    TrangThai bit DEFAULT 1,
    CONSTRAINT UK_NhaCungCap_Email UNIQUE(Email)
);

-- 7. Phiếu nhập
CREATE TABLE PhieuNhap (
    MaPN varchar(10) PRIMARY KEY,
    MaNV varchar(10) NOT NULL,
    MaNCC varchar(10) NOT NULL,
    NgayTao date DEFAULT GETDATE(),
    CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_PhieuNhap_NhaCungCap FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC)
);

-- 8. Chi tiết phiếu nhập
CREATE TABLE ChiTietPhieuNhap (
    MaCTPN varchar(10) PRIMARY KEY,
    MaPN varchar(10) NOT NULL,
    MaSP varchar(10) NOT NULL,
    SoluongNhap int NOT NULL CHECK (SoluongNhap > 0),
    DonGiaNhap decimal(18,2) NOT NULL CHECK (DonGiaNhap >= 0),
    CONSTRAINT FK_ChiTietPN_Phieu FOREIGN KEY (MaPN) REFERENCES PhieuNhap(MaPN) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietPN_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Tạo một số chỉ mục cần thiết
CREATE INDEX IX_SanPham_TenSP ON SanPham(TenSP);
CREATE INDEX IX_KhachHang_SDT ON KhachHang(SDT);
CREATE INDEX IX_PhieuBanHang_NgayTao ON PhieuBanHang(NgayTao);
go

CREATE OR ALTER TRIGGER trg_NhapKho_Insert
ON ChiTietPhieuNhap
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    MERGE SanPham AS t
    USING (
        SELECT MaSP, SUM(SoLuongNhap) AS SL
       	FROM inserted
        GROUP BY MaSP
    ) AS i ON t.MaSP = i.MaSP
    WHEN MATCHED THEN
        UPDATE SET SoLuongTon = SoLuongTon + i.SL
    WHEN NOT MATCHED THEN
        INSERT (MaSP, SoLuongTon) VALUES (i.MaSP, i.SL);
END

go

CREATE OR ALTER TRIGGER trg_NhapKho_Update
ON ChiTietPhieuNhap
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ chạy khi cột SoLuongNhap hoặc MaSP thực sự bị thay đổi
    IF NOT (UPDATE(SoLuongNhap) OR UPDATE(MaSP))
        RETURN;

    -- Bước 1: Trừ đi số lượng cũ (deleted)
    MERGE SanPham AS t
    USING (
        SELECT MaSP, SUM(SoLuongNhap) AS SL
        FROM deleted
        GROUP BY MaSP
    ) AS d ON t.MaSP = d.MaSP
    WHEN MATCHED THEN
        UPDATE SET SoLuongTon = SoLuongTon - d.SL;

    -- Bước 2: Cộng thêm số lượng mới (inserted)
    MERGE SanPham AS t
    USING (
        SELECT MaSP, SUM(SoLuongNhap) AS SL
        FROM inserted
        GROUP BY MaSP
    ) AS i ON t.MaSP = i.MaSP
    WHEN MATCHED THEN
        UPDATE SET SoLuongTon = SoLuongTon + i.SL
    WHEN NOT MATCHED THEN
        INSERT (MaSP, SoLuongTon) VALUES (i.MaSP, i.SL);
END

go

CREATE OR ALTER TRIGGER trg_NhapKho_Delete
ON ChiTietPhieuNhap
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    MERGE SanPham AS t
    USING (
        SELECT MaSP, SUM(SoLuongNhap) AS SL
        FROM deleted
        GROUP BY MaSP
    ) AS d ON t.MaSP = d.MaSP
    WHEN MATCHED THEN
        UPDATE SET SoLuongTon = SoLuongTon - d.SL;

    -- (Tùy chính sách) Nếu số lượng về 0 thì có thể xóa bản ghi luôn
    -- DELETE FROM SanPham WHERE SoLuongTon <= 0;
END
go

-- ============================================================
-- 1. TRIGGER INSERT CHI TIẾT BÁN HÀNG (đã đúng, chỉ sửa nhỏ cho chắc)
-- ============================================================
CREATE OR ALTER TRIGGER trg_BanHang_Insert
ON ChiTietPhieuBanHang
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Nếu có ít nhất 1 phiếu chưa thanh toán → không trừ tồn
    IF EXISTS (SELECT 1 FROM PhieuBanHang p 
               JOIN inserted i ON p.MaPBH = i.MaPBH 
               WHERE p.TrangThai = 0)
        RETURN;

    -- Kiểm tra tồn kho
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN SanPham sp ON i.MaSP = sp.MaSP
        GROUP BY sp.MaSP, sp.SoLuongTon
        HAVING sp.SoLuongTon < SUM(i.SoLuongBan)
    )
    BEGIN
        RAISERROR(N'Số lượng bán vượt quá tồn kho hiện có!', 16, 1);
        ROLLBACK;
        RETURN;
    END

    -- Trừ tồn kho
    UPDATE SanPham
    SET SoLuongTon = SoLuongTon - i.TongBan
    FROM SanPham sp
    JOIN (
        SELECT MaSP, SUM(SoLuongBan) AS TongBan
        FROM inserted
        GROUP BY MaSP
    ) i ON sp.MaSP = i.MaSP;
END
GO


-- ============================================================
-- 2. TRIGGER UPDATE CHI TIẾT BÁN HÀNG (đã sửa hoàn toàn lỗi MaPBH)
-- ============================================================
CREATE OR ALTER TRIGGER trg_BanHang_Update
ON ChiTietPhieuBanHang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Nếu không thay đổi số lượng hoặc mã sản phẩm → bỏ qua
    IF NOT (UPDATE(SoLuongBan) OR UPDATE(MaSP))
        RETURN;

    -- Lấy danh sách các phiếu bị ảnh hưởng + trạng thái thanh toán
    ;WITH Affected AS (
        SELECT DISTINCT MaPBH 
        FROM inserted
        UNION
        SELECT DISTINCT MaPBH 
        FROM deleted
    ),
    PaidOrders AS (
        SELECT a.MaPBH
        FROM Affected a
        JOIN PhieuBanHang p ON p.MaPBH = a.MaPBH
        WHERE p.TrangThai = 1
    )

    -- 1. Cộng lại số lượng cũ (deleted) nếu phiếu đã thanh toán
    UPDATE sp
    SET SoLuongTon = sp.SoLuongTon + d.S
    FROM SanPham sp
    JOIN (
        SELECT MaSP, SUM(SoLuongBan) AS S
        FROM deleted d
        WHERE EXISTS (SELECT 1 FROM PaidOrders po WHERE po.MaPBH = d.MaPBH)
        GROUP BY MaSP
    ) d ON sp.MaSP = d.MaSP;

    -- 2. Kiểm tra tồn kho trước khi trừ số lượng mới
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN SanPham sp ON i.MaSP = sp.MaSP
        WHERE EXISTS (SELECT 1 FROM PaidOrders po WHERE po.MaPBH = i.MaPBH)
        GROUP BY i.MaSP, sp.SoLuongTon
        HAVING sp.SoLuongTon < SUM(i.SoLuongBan)
    )
    BEGIN
        RAISERROR(N'Số lượng bán sau khi sửa vượt quá tồn kho!', 16, 1);
        ROLLBACK;
        RETURN;
    END

    -- 3. Trừ số lượng mới (inserted) nếu phiếu đã thanh toán
    UPDATE sp
    SET SoLuongTon = sp.SoLuongTon - i.S
    FROM SanPham sp
    JOIN (
        SELECT MaSP, SUM(SoLuongBan) AS S
        FROM inserted i
        WHERE EXISTS (SELECT 1 FROM PaidOrders po WHERE po.MaPBH = i.MaPBH)
        GROUP BY MaSP
    ) i ON sp.MaSP = i.MaSP;

END
GO


-- ============================================================
-- 3. TRIGGER DELETE CHI TIẾT BÁN HÀNG (sửa lỗi GO và cú pháp)
-- ============================================================
DROP TRIGGER IF EXISTS trg_BanHang_Delete;
GO

CREATE OR ALTER TRIGGER trg_BanHang_Delete
ON ChiTietPhieuBanHang
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE sp
    SET SoLuongTon = sp.SoLuongTon + d.S
    FROM SanPham sp
    JOIN (
        SELECT MaSP, SUM(SoLuongBan) AS S
        FROM deleted d
        WHERE EXISTS (
            SELECT 1 FROM PhieuBanHang p 
            WHERE p.MaPBH = d.MaPBH AND p.TrangThai = 1
        )
        GROUP BY MaSP
    ) d ON sp.MaSP = d.MaSP;
END
GO


-- ============================================================
-- 4. TRIGGER THANH TOÁN PHIẾU (khi đổi TrangThai 0 → 1)
-- ============================================================
CREATE OR ALTER TRIGGER trg_ThanhToan_PhieuBanHang
ON PhieuBanHang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(TrangThai) RETURN;

    DECLARE @MaPBH varchar(10);

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT i.MaPBH
        FROM inserted i
        INNER JOIN deleted d ON i.MaPBH = d.MaPBH
        WHERE d.TrangThai = 0 AND i.TrangThai = 1;

    OPEN cur;
    FETCH NEXT FROM cur INTO @MaPBH;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Kiểm tra tồn kho
        IF EXISTS (
            SELECT 1
            FROM ChiTietPhieuBanHang ct
            JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE ct.MaPBH = @MaPBH
            GROUP BY sp.MaSP, sp.SoLuongTon
            HAVING sp.SoLuongTon < SUM(ct.SoLuongBan)
        )
        BEGIN
            RAISERROR(N'Phiếu %s: Không đủ hàng trong kho để thanh toán!', 16, 1, @MaPBH);
            ROLLBACK;
            CLOSE cur; DEALLOCATE cur;
            RETURN;
        END

        -- Trừ tồn kho
        UPDATE SanPham
        SET SoLuongTon = SoLuongTon - ct.SL
        FROM SanPham sp
        JOIN (
            SELECT MaSP, SUM(SoLuongBan) AS SL
            FROM ChiTietPhieuBanHang
            WHERE MaPBH = @MaPBH
            GROUP BY MaSP
        ) ct ON sp.MaSP = ct.MaSP;

        FETCH NEXT FROM cur INTO @MaPBH;
    END

    CLOSE cur;
    DEALLOCATE cur;
END
GO
-- Tự động tính lại tổng tiền mỗi khi thêm/sửa/xóa chi tiết phiếu bán
CREATE OR ALTER TRIGGER trg_CapNhatTongTien_PBH
ON ChiTietPhieuBanHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    WITH CTE AS (
        SELECT MaPBH, 
               SUM(SoLuongBan * DonGia) AS Tong
        FROM (
            SELECT MaPBH, SoLuongBan, DonGia FROM inserted
            UNION ALL
            SELECT MaPBH, -SoLuongBan, DonGia FROM deleted
        ) t
        GROUP BY MaPBH
    )
    UPDATE p
    SET TongTien = ISNULL((
        SELECT SUM(SoLuongBan * DonGia)
        FROM ChiTietPhieuBanHang ct
        WHERE ct.MaPBH = p.MaPBH
    ), 0)
    FROM PhieuBanHang p
    WHERE p.MaPBH IN (SELECT MaPBH FROM CTE);
END
GO

-- Không cho sửa MaSP trong ChiTietPhieuBanHang (nên tạo mới dòng thay vì sửa)
CREATE OR ALTER TRIGGER trg_KhongSuaMaSP_ChiTietBan
ON ChiTietPhieuBanHang
FOR UPDATE
AS
BEGIN
    IF UPDATE(MaSP)
    BEGIN
        RAISERROR(N'Không được phép sửa mã sản phẩm trong chi tiết phiếu bán. Hãy xóa và thêm mới!', 16, 1);
        ROLLBACK;
    END
END
GO

USE ASM_Net107
GO

-- =============================================================
-- 1. Nhân viên (3 nhân viên: admin, quản lý, nhân viên)
-- =============================================================
INSERT INTO NhanVien (MaNV, TenNV, Email, SDT, TenDangNhap, MatKhau, ChucVu, TrangThai)
VALUES 
('NV001', N'Nguyễn Văn Admin', 'admin@shop.com', '0901234567', 'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Quản lý', 1), -- pass: admin123
('NV002', N'Trần Thị Hương', 'huong@shop.com', '0912345678', 'nhanvien1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Nhân viên', 1), -- pass: 123456
('NV003', N'Lê Văn Nam', 'nam@shop.com', '0923456789', 'nhanvien2', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'Nhân viên', 1);
-- Mật khẩu trên đã được hash SHA-256 của chuỗi "123456"

-- =============================================================
-- 2. Nhà cung cấp (5 nhà cung cấp)
-- =============================================================
INSERT INTO NhaCungCap (MaNCC, TenNCC, Email, SDT)
VALUES
('NCC01', N'Công ty TNHH Thực phẩm Sạch VinEco', 'vineco@gmail.com', '0988111222'),
('NCC02', N'Công ty CP Sữa Việt Nam Vinamilk', 'vinamilk@vinamilk.com.vn', '0285415522'),
('NCC03', N'Công ty Bia Sài Gòn SABECO', 'sabeco@sabeco.com.vn', '0283829526'),
('NCC04', N'Công ty CP Acecook Việt Nam', 'acecook@acecook.vn', '02838984567'),
('NCC05', N'Công ty TNHH Nước giải khát Suntory PepsiCo', 'pepsi@pepsi.vn', '02839104999');

-- =============================================================
-- 3. Sản phẩm (20 sản phẩm để dư dả)
-- =============================================================
INSERT INTO SanPham (MaSP, TenSP, DonGia, SoLuongTon)
VALUES
('SP001', N'Sữa tươi Vinamilk không đường 1L', 32000, 150),
('SP002', N'Sữa chua Vinamilk có đường hộp 100g', 6000, 300),
('SP003', N'Bia Sài Gòn Export lon 330ml', 14500, 500),
('SP004', N'Bia Heineken bạc lon 330ml', 22500, 400),
('SP005', N'Mì Hảo Hảo tôm chua cay', 4500, 1000),
('SP006', N'Mì Omni thịt bò gói 75g', 5500, 800),
('SP007', N'Pepsi lon 330ml', 10000, 600),
('SP008', N'Coca Cola lon 330ml', 11000, 550),
('SP009', N'Nước suối Aquafina 500ml', 6000, 700),
('SP010', N'Trà xanh C2 chai 455ml', 8500, 400),
('SP011', N'Bánh Oreo vị socola 120g', 18500, 250),
('SP012', N'Snicker thanh 51g', 14000, 300),
('SP013', N'Gạo ST25 túi 5kg', 145000, 80),
('SP014', N'Dầu ăn Simply đậu nành 5L', 185000, 60),
('SP015', N'Đường trắng Biên Hòa 1kg', 24000, 200),
('SP016', N'Nước mắm Phú Quốc 35 độ đạm 520ml', 42000, 150),
('SP017', N'Muối i-ốt Vĩ Đại 1kg', 8500, 300),
('SP018', N'Bột ngọt Ajinomoto 1kg', 68000, 100),
('SP019', N'Tương ớt Chinsu 250g', 18000, 400),
('SP020', N'Nui ống Safoco 400g', 16000, 250);

-- =============================================================
-- 4. Khách hàng (10 khách hàng)
-- =============================================================
INSERT INTO KhachHang (MaKH, TenKH, SDT)
VALUES
('KH001', N'Nguyễn Thị Lan', '0905123456'),
('KH002', N'Trần Văn Hùng', '0918234567'),
('KH003', N'Phạm Minh Tuấn', '0934567890'),
('KH004', N'Lê Thị Mai', '0945678901'),
('KH005', N'Vũ Văn Hoàng', '0956789012'),
('KH006', N'Hoàng Thị Ngọc', '0967890123'),
('KH007', N'Đỗ Văn Khánh', '0978901234'),
('KH008', N'Bùi Thị Hà', '0989012345'),
('KH009', N'Khách lẻ 001', '0391234567'),
('KH010', N'Khách lẻ 002', '0392345678');

-- =============================================================
-- 5. Phiếu nhập hàng (2 phiếu nhập mẫu)
-- =============================================================
INSERT INTO PhieuNhap (MaPN, MaNV, MaNCC, NgayTao)
VALUES
('PN001', 'NV001', 'NCC02', '2025-10-15'),
('PN002', 'NV002', 'NCC03', '2025-11-01');

-- Chi tiết phiếu nhập PN001 (sữa Vinamilk)
INSERT INTO ChiTietPhieuNhap (MaCTPN, MaPN, MaSP, SoluongNhap, DonGiaNhap)
VALUES
('CTPN001', 'PN001', 'SP001', 100, 28000),
('CTPN002', 'PN001', 'SP002', 200, 4500);

-- Chi tiết phiếu nhập PN002 (bia)
INSERT INTO ChiTietPhieuNhap (MaCTPN, MaPN, MaSP, SoluongNhap, DonGiaNhap)
VALUES
('CTPN003', 'PN002', 'SP003', 300, 12000),
('CTPN004', 'PN002', 'SP004', 200, 19000);

-- =============================================================
-- 6. Phiếu bán hàng (5 phiếu: có cả đã thanh toán và chưa thanh toán)
-- =============================================================
-- Phiếu 001 - Đã thanh toán
INSERT INTO PhieuBanHang (MaPBH, MaKH, MaNV, TrangThai, NgayTao, TongTien)
VALUES ('PBH001', 'KH001', 'NV002', 1, '2025-11-10', NULL);

INSERT INTO ChiTietPhieuBanHang (MaCTPBH, MaPBH, MaSP, DonGia, SoLuongBan)
VALUES
('CT001', 'PBH001', 'SP001', 32000, 5),
('CT002', 'PBH001', 'SP003', 14500, 12),
('CT003', 'PBH001', 'SP007', 10000, 6);

-- Phiếu 002 - Đã thanh toán
INSERT INTO PhieuBanHang (MaPBH, MaKH, MaNV, TrangThai, NgayTao, TongTien)
VALUES ('PBH002', 'KH003', 'NV003', 1, '2025-11-12', NULL);

INSERT INTO ChiTietPhieuBanHang (MaCTPBH, MaPBH, MaSP, DonGia, SoLuongBan)
VALUES
('CT004', 'PBH002', 'SP005', 4500, 20),
('CT005', 'PBH002', 'SP008', 11000, 10),
('CT006', 'PBH002', 'SP011', 18500, 5);

-- Phiếu 003 - Chưa thanh toán (đang xử lý)
INSERT INTO PhieuBanHang (MaPBH, MaKH, MaNV, TrangThai, NgayTao)
VALUES ('PBH003', 'KH005', 'NV002', 0, '2025-11-18');

INSERT INTO ChiTietPhieuBanHang (MaCTPBH, MaPBH, MaSP, DonGia, SoLuongBan)
VALUES
('CT007', 'PBH003', 'SP004', 22500, 10),
('CT008', 'PBH003', 'SP009', 6000, 24);

-- Phiếu 004 - Đã thanh toán
INSERT INTO PhieuBanHang (MaPBH, MaKH, MaNV, TrangThai, NgayTao, TongTien)
VALUES ('PBH004', 'KH002', 'NV001', 1, '2025-11-17', NULL);

INSERT INTO ChiTietPhieuBanHang (MaCTPBH, MaPBH, MaSP, DonGia, SoLuongBan)
VALUES
('CT009', 'PBH004', 'SP013', 145000, 2),
('CT010', 'PBH004', 'SP014', 185000, 1);

-- Phiếu 005 - Chưa thanh toán
INSERT INTO PhieuBanHang (MaPBH, MaKH, MaNV, TrangThai, NgayTao)
VALUES ('PBH005', 'KH008', 'NV003', 0, '2025-11-18');

INSERT INTO ChiTietPhieuBanHang (MaCTPBH, MaPBH, MaSP, DonGia, SoLuongBan)
VALUES
('CT011', 'PBH005', 'SP006', 5500, 30),
('CT012', 'PBH005', 'SP019', 18000, 8);

-- =============================================================
-- Kiểm tra tổng tiền tự động (trigger sẽ cập nhật)
-- =============================================================
SELECT MaPBH, TongTien FROM PhieuBanHang ORDER BY MaPBH;

-- Kiểm tra tồn kho hiện tại
SELECT MaSP, TenSP, SoLuongTon FROM SanPham ORDER BY MaSP;

-- Kiểm tra trigger thanh toán: thử thanh toán phiếu PBH003
-- UPDATE PhieuBanHang SET TrangThai = 1 WHERE MaPBH = 'PBH003'
-- Nếu tồn đủ thì sẽ thành công và trừ tồn kho