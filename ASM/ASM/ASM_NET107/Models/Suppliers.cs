using System.Numerics;

namespace ASM_NET107.Models
{
    public class Suppliers
    {
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
