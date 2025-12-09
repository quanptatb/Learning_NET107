using System.Data;
using System.Security.Cryptography.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM_NET107.Models
{
    public class PurchaseInvoices
    {
        public string PurchaseID { get; set; }
        public string EmployeeID { get; set; }
        public string SupplierID { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
