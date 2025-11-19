using System.Data;
using System.Text.RegularExpressions;

namespace ASM.Models
{
    public class NhaCungCap
    {
//        -- 6. Nhà cung cấp
//CREATE TABLE NhaCungCap(
//    MaNCC varchar(10) PRIMARY KEY,
//    TenNCC nvarchar(100) NOT NULL,
//    Email varchar(100),
//    SDT varchar(15),
//    NgayTaoTK date DEFAULT GETDATE(),
//    TrangThai bit DEFAULT 1,
//    CONSTRAINT UK_NhaCungCap_Email UNIQUE(Email)
//);
        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public DateTime NgayTaoTK { get; set; }
        public bool TrangThai { get; set; }
    }
}
