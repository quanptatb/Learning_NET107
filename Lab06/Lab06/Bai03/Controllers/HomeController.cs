using Bai03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai03.Controllers
{
    // 1. SỬA: Chỉ để 1 Route duy nhất cho Controller để tránh xung đột
    [Route("Home")]
    public class HomeController : Controller
    {
        private static List<ProductModel> products = new List<ProductModel>()
        {
            new ProductModel { Name = "Men Shoes", Price = 99, Quantity = 100, ID = 1},
            new ProductModel { Name = "Women Shoes", Price = 199, Quantity = 200, ID = 2}
        };
        [HttpGet("~/")]
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View(products);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel model)
        {
            var id = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;  
            model.ID = id;
            products.Add(model);
            return View(model);
        }

        [HttpGet("Edit/{name:regex(^[[^/]]+$)}")]
        public IActionResult Edit(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost("Edit/{name}")]
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

        [HttpGet("Delete/{name:regex(^[[^/]]+$)}")]
        public IActionResult Delete(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(string name)
        {
            var product = products.FirstOrDefault(p => p.Name == name);
            if (product != null)
            {
                products.Remove(product);
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Details
        [HttpGet("Details/{id:regex()int")]
        public IActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.ID == id);
            if (product == null) return NotFound();
            //không phải số 
            return View(product);
        }
    }
}
