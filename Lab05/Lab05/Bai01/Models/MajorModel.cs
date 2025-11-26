using Microsoft.AspNetCore.Mvc;

namespace Bai01.Models
{
    public class MajorModel
    {
        public string Image_string { get; set; } = "";
        public string Subname_string { get; set; } = "";

        // Method trả về danh sách chuyên ngành CNTT (dữ liệu mẫu)
        public static List<MajorModel> GetMajors() => new()
        {
            new() { Image_string="/images/1.png",
                    Subname_string="Thiết kế đồ họa" },

            new() { Image_string="/images/2.png",
                    Subname_string="Lập trình máy tính – Thiết bị di động" },

            new() { Image_string="/images/3.png",
                    Subname_string="Thiết kế Website" },

            new() { Image_string="/images/4.png",
                    Subname_string="CNTT – Ứng dụng phần mềm" }
        };
    }
}
