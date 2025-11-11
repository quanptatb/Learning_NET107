namespace Bai02.Models
{
    public class IphoneModel
    {
        public string Images_String { get; set; } = "";
        public string Productname_string { get; set; } = "";
        public string Price_string { get; set; } = "";
        public string Rating_string { get; set; } = "";
        public string Screen_string { get; set; } = "";
        public string Camera_string { get; set; } = "";
        public string Battery_string { get; set; } = "";
        public string Ram_string { get; set; } = "";
        public string CPU_string { get; set; } = "";
        public string Operatingsystem_string { get; set; } = "";
        // Method trả về danh sách iPhone (dữ liệu mẫu)
        public static List<IphoneModel> GetIphones() => new()
        {
            // 11 pro max 64gb
            new()
            {
                Images_String = "/images/iphone11promax.png",
                Productname_string = "iPhone 11 Pro Max 64GB",
                Price_string = "22.990.000 ₫",
                Rating_string = "4.8",
                Screen_string = "6.5 inch, OLED, 1242 x 2688 Pixels",
                Camera_string = "Chính 12 MP & Phụ 12 MP, Selfie 12 MP",
                Battery_string = "3969 mAh",
                Ram_string = "4 GB",
                CPU_string = "Apple A13 Bionic 6 nhân",
                Operatingsystem_string = "iOS 13"
            },
            // 11 pro max 256gb
            new()
            {
                Images_String = "/images/iphone11promax256gb.png",
                Productname_string = "iPhone 11 Pro Max 256GB",
                Price_string = "25.990.000 ₫",
                Rating_string = "4.8",
                Screen_string = "6.5 inch, OLED, 1242 x 2688 Pixels",
                Camera_string = "Chính 12 MP & Phụ 12 MP, Selfie 12 MP",
                Battery_string = "3969 mAh",
                Ram_string = "4 GB",
                CPU_string = "Apple A13 Bionic 6 nhân",
                Operatingsystem_string = "iOS 13"
            },
            // 11 pro max 512gb
            new()
            {
                Images_String = "/images/iphone11promax512gb.png",
                Productname_string = "iPhone 11 Pro Max 512GB",
                Price_string = "30.990.000 ₫",
                Rating_string = "4.8",
                Screen_string = "6.5 inch, OLED, 1242 x 2688 Pixels",
                Camera_string = "Chính 12 MP & Phụ 12 MP, Selfie 12 MP",
                Battery_string = "3969 mAh",
                Ram_string = "4 GB",
                CPU_string = "Apple A13 Bionic 6 nhân",
                Operatingsystem_string = "iOS 13"
            },
            // 11 pro 64gb
            new()
            {
                Images_String = "/images/iphone11pro.png",
                Productname_string = "iPhone 11 Pro 64GB",
                Price_string = "20.990.000 ₫",
                Rating_string = "4.8",
                Screen_string = "5.8 inch, OLED, 1125 x 2436 Pixels",
                Camera_string = "Chính 12 MP & Phụ 12 MP, Selfie 12 MP",
                Battery_string = "3046 mAh",
                Ram_string = "4 GB",
                CPU_string = "Apple A13 Bionic 6 nhân",
                Operatingsystem_string = "iOS 13"
            }
        };
    }
}
