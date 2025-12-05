using Bai01.DAL;
using Bai01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai01.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDAL _productDAL;

        public HomeController(ProductDAL productDAL)
        {
            _productDAL = productDAL;
        }

        // 1. Hiển thị danh sách (Index)
        public IActionResult Index()
        {
            var products = _productDAL.GetAllProducts();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        // Xử lý Tạo mới (Create - POST)
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productDAL.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // 3. Trang Chỉnh sửa (Edit - GET)
        public IActionResult Edit(int id)
        {
            var product = _productDAL.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý Chỉnh sửa (Edit - POST)
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productDAL.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // 4. Trang Chi tiết (Details)
        public IActionResult Details(int id)
        {
            var product = _productDAL.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // 5. Trang Xóa (Delete - GET)
        public IActionResult Delete(int id)
        {
            var product = _productDAL.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý Xóa (Delete - POST)
        [HttpPost("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productDAL.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}