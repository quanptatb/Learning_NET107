using Bai02.DAL;
using Bai02.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai02.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDAL _productDAL;

        public HomeController(ProductDAL productDAL)
        {
            _productDAL = productDAL;
        }

        // Action hiển thị danh sách (Yêu cầu 1)
        public IActionResult Index()
        {
            List<Product> products = _productDAL.GetAllProducts();
            return View(products);
        }

        // Action hiển thị chi tiết (Yêu cầu 2)
        public IActionResult Details(int id)
        {
            Product product = _productDAL.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
