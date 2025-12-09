using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM_NET107.Models
{
    public class Customers
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
