namespace ASM_NET107.Models
{
    public class Products
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageURL { get; set; }
    }
}
