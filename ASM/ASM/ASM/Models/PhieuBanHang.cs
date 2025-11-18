using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Data;
using System.Security.Cryptography.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM.Models
{
    public class PhieuBanHang
    {
//        -- 4. Phiếu bán hàng
//CREATE TABLE PhieuBanHang(
//    MaPBH varchar(10) PRIMARY KEY,
//    MaKH varchar(10) NOT NULL,
//    MaNV varchar(10) NOT NULL,
//    TrangThai bit default 0,           -- 1: đã thanh toán, 0: đang xử lý/hủy
//    NgayTao date DEFAULT GETDATE(),
//    TongTien decimal (18,2) NULL,       -- có thể tính bằng trigger hoặc view
//    CONSTRAINT FK_PhieuBanHang_KhachHang FOREIGN KEY(MaKH) REFERENCES KhachHang(MaKH),
//    CONSTRAINT FK_PhieuBanHang_NhanVien FOREIGN KEY(MaNV) REFERENCES NhanVien(MaNV)
//);
        public string MaPBH { get; set; }
        public string MaKH { get; set; }
        public string MaNV { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
    }
}
