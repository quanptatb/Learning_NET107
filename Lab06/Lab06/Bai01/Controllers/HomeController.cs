using Bai01.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Bai01.Controllers
{
    public class HomeController : Controller
    {
        private static List<ProductModel> products = new List<ProductModel>()
    {
        new ProductModel { Name = "Men Shoes", Price = 99, Quantity = 100 },
        new ProductModel { Name = "Women Shoes", Price = 199, Quantity = 200 }
    };
        public IActionResult Index()
        {
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductModel model)
        {
            products.Add(model);
            return RedirectToAction("Index");
        }
        public IActionResult Edit(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(ProductModel model)
        {
            var product = products.FirstOrDefault(p => p.Name == model.Name);
            if (product != null)
            {
                product.Price = model.Price;
                product.Quantity = model.Quantity;
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            return View(product);
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            if (product != null)
            {
                products.Remove(product);
            }
            return RedirectToAction("Index");
        }
    }
}