using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM_NET107.Models
{
    public class SalesInvoices
    {
        public string InvoiceID { get; set; }
        public string CustomerID { get; set; }
        public string EmployeeID { get; set; }
        public bool Status { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
