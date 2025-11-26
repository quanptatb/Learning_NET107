using Microsoft.AspNetCore.Mvc;

namespace Bai02.Models
{
    public class MajorModel
    {
        public string Title_string { get; set; } = "";
        public string Image_string { get; set; } = "";
        public string Subname_string { get; set; } = "";
        // Method trả về danh sách chuyên ngành CNTT (dữ liệu mẫu)
        public static List<MajorModel> GetMajors() => new()
        {
            new() { Title_string="Công nghệ thông tin",
                    Image_string="/images/1.png",
                    Subname_string="Thiết kế đồ họa" },

            new() { Title_string="Công nghệ thông tin",
                    Image_string="/images/2.png",
                    Subname_string="Lập trình máy tính – Thiết bị di động" },

            new() { Title_string="Công nghệ thông tin",
                    Image_string="/images/3.png",
                    Subname_string="Thiết kế Website" },

            new() { Title_string="Công nghệ thông tin",
                    Image_string="/images/4.png",
                    Subname_string="CNTT – Ứng dụng phần mềm" }
        };
    }
}
