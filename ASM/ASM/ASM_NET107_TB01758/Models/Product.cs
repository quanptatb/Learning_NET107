using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;
using System.Security.Cryptography.Xml;
using System.Security.Principal;

namespace ASM_NET107_TB01758.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
