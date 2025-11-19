using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM.Models
{
    public class KhachHang
    {
//        -- 3. Khách hàng
//CREATE TABLE KhachHang(
//    MaKH varchar(10) PRIMARY KEY,
//    TenKH nvarchar(50) NOT NULL,
//    SDT varchar(15) NOT NULL,
//    NgayTaoTK date DEFAULT GETDATE(),
//    CONSTRAINT UK_KhachHang_SDT UNIQUE(SDT)
//);
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string SDT { get; set; }
        public DateTime NgayTaoTK { get; set; }
    }
}
