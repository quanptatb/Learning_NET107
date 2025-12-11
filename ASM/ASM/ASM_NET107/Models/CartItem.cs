namespace ASM_NET107.Models
{
    public class CartItem
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }
}