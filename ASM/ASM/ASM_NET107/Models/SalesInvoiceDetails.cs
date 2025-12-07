namespace ASM_NET107.Models
{
    public class SalesInvoiceDetails
    {
        public string DetailID { get; set; }
        public string InvoiceID { get; set; }
        public string ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
