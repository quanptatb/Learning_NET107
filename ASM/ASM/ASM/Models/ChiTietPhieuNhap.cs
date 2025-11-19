using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Security.Cryptography.Xml;

namespace ASM.Models
{
    public class ChiTietPhieuNhap
    {
//        -- 8. Chi tiết phiếu nhập
//CREATE TABLE ChiTietPhieuNhap(
//    MaCTPN varchar(10) PRIMARY KEY,
//    MaPN varchar(10) NOT NULL,
//    MaSP varchar(10) NOT NULL,
//    SoluongNhap int NOT NULL CHECK(SoluongNhap > 0),
//    DonGiaNhap decimal (18,2) NOT NULL CHECK(DonGiaNhap >= 0),
//    CONSTRAINT FK_ChiTietPN_Phieu FOREIGN KEY(MaPN) REFERENCES PhieuNhap(MaPN) ON DELETE CASCADE,
//    CONSTRAINT FK_ChiTietPN_SanPham FOREIGN KEY(MaSP) REFERENCES SanPham(MaSP)
//);
        public string MaCTPN { get; set; }
        public string MaPN { get; set; }
        public string MaSP { get; set; }
        public int SoluongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
    }
}
