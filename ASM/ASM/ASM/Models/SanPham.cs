using Microsoft.AspNetCore.Http.HttpResults;

namespace ASM.Models
{
    public class SanPham
    {
//        CREATE TABLE SanPham(
//    MaSP varchar(10) PRIMARY KEY,
//    TenSP nvarchar(100) NOT NULL,
//    DonGia decimal (18,2) NOT NULL CHECK(DonGia >= 0),
//    SoLuongTon int NOT NULL DEFAULT 0 CHECK(SoLuongTon >= 0)
//);
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }
    }
}
