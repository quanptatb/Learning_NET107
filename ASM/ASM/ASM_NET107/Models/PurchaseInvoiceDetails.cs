namespace ASM_NET107.Models
{
    public class PurchaseInvoiceDetails
    {
        public string PurchaseDetailID { get; set; }
        public string PurchaseID { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
