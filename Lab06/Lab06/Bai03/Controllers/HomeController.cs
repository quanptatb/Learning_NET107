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
            new ProductModel { Name = "Men Shoes", Price = 99, Quantity = 100},
            new ProductModel { Name = "Women Shoes", Price = 199, Quantity = 200}
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
            if (ModelState.IsValid)
            {
                products.Add(model);
                return RedirectToAction("Index");
            }
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

        [HttpGet("Details/{name:regex(^[^/]+$)}")]
        public IActionResult Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Product name is required.");

            var product = products.FirstOrDefault(p =>
                p.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
