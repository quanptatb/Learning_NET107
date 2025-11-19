using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM.Models
{
    public class NhanVien
    {
        //-- 1. Nhân viên
        //CREATE TABLE NhanVien(
        //    MaNV varchar(10) PRIMARY KEY,
        //    TenNV nvarchar(50) NOT NULL,
        //    Email varchar(100) NOT NULL,
        //    SDT varchar(15),
        //    TenDangNhap varchar(20) NOT NULL,
        //    MatKhau varchar(255) NOT NULL,  -- phải hash khi dùng thật
        //    ChucVu nvarchar(20) DEFAULT N'Nhân viên',
        //    NgayTaoTK date DEFAULT GETDATE(),
        //    TrangThai bit DEFAULT 1,
        //    CONSTRAINT UK_NhanVien_Email UNIQUE(Email),
        //    CONSTRAINT UK_NhanVien_TenDangNhap UNIQUE(TenDangNhap),
        //    CONSTRAINT CK_NhanVien_SDT CHECK(SDT LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' AND LEN(SDT) IN (10,11))
        //);
        [Key]
        public string MaNV { get; set; }

        [Required(ErrorMessage = "Tên nhân viên không được để trống")]
        public string TenNV { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string SDT { get; set; }

        [Required]
        public string TenDangNhap { get; set; }

        [Required]
        public string MatKhau { get; set; } // Lưu ý: Thực tế nên Hash mật khẩu

        public string ChucVu { get; set; }
        public DateTime NgayTaoTK { get; set; }
        public bool TrangThai { get; set; }
    }
}
