using System;
using System.ComponentModel.DataAnnotations;

namespace Bai01.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}