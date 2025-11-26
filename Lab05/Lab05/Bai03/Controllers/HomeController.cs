using Bai03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai03.Controllers
{
    public class HomeController : Controller
    {
        private static List<ProductEditModel> products = new List<ProductEditModel>()
    {
        new ProductEditModel { Name = "Men Shoes", Price = 99, Quantity = 100 },
        new ProductEditModel { Name = "Women Shoes", Price = 199, Quantity = 200 }
    };

        public IActionResult Index()
        {
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductEditModel model)
        {
            products.Add(model);
            return RedirectToAction("Index");
        }
    }
}
