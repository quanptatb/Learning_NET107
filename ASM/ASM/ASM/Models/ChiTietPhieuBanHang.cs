using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Security.Cryptography.Xml;

namespace ASM.Models
{
    public class ChiTietPhieuBanHang
    {
//        -- 5. Chi tiết phiếu bán
//CREATE TABLE ChiTietPhieuBanHang(
//    MaCTPBH varchar(10) PRIMARY KEY,
//    MaPBH varchar(10) NOT NULL,
//    MaSP varchar(10) NOT NULL,
//    DonGia decimal (18,2) NOT NULL,
//    SoLuongBan int NOT NULL CHECK(SoLuongBan > 0),
//    CONSTRAINT FK_ChiTietPBH_Phieu FOREIGN KEY(MaPBH) REFERENCES PhieuBanHang(MaPBH) ON DELETE CASCADE,
//    CONSTRAINT FK_ChiTietPBH_SanPham FOREIGN KEY(MaSP) REFERENCES SanPham(MaSP)
//);
        public string MaCTPBH { get; set; }
        public string MaPBH { get; set; }
        public string MaSP { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuongBan { get; set; }
    }
}
