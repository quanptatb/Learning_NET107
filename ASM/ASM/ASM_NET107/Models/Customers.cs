using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM_NET107.Models
{
    public class Customers
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; } // Thêm trường này
        public DateOnly CreatedDate { get; set; }
    }
}
