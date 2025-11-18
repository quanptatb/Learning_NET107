using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM.Models
{
    public class PhieuNhap
    {
//        -- 7. Phiếu nhập
//CREATE TABLE PhieuNhap(
//    MaPN varchar(10) PRIMARY KEY,
//    MaNV varchar(10) NOT NULL,
//    MaNCC varchar(10) NOT NULL,
//    NgayTao date DEFAULT GETDATE(),
//    CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY(MaNV) REFERENCES NhanVien(MaNV),
//    CONSTRAINT FK_PhieuNhap_NhaCungCap FOREIGN KEY(MaNCC) REFERENCES NhaCungCap(MaNCC)
//);
        public string MaPN { get; set; }
        public string MaNV { get; set; }
        public string MaNCC { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
