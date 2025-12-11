using ASM_NET107.DAL;
using ASM_NET107.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NET107.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDAL;
        public ProductController(ProductDAL productDAL) => _productDAL = productDAL;

        // Hiển thị danh sách (Cho cả Admin và Khách)
        public IActionResult Index(string searchString)
        {
            List<Products> data;
            if (!string.IsNullOrEmpty(searchString))
            {
                data = _productDAL.SearchProducts(searchString);
            }
            else
            {
                data = _productDAL.GetProducts();
            }
            ViewData["CurrentFilter"] = searchString; // Để giữ lại từ khóa trên ô tìm kiếm
            return View(data);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Manager" && role != "Staff") // Kiểm tra cả 2 quyền
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Products p)
        {
            if (ModelState.IsValid)
            {
                _productDAL.InsertProduct(p);
                return RedirectToAction("Index");
            }
            return View(p);
        }
        // Tương tự cho Edit, Delete...
        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var product = _productDAL.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Products p)
        {
            if (ModelState.IsValid)
            {
                _productDAL.UpdateProduct(p);
                return RedirectToAction("Index");
            }
            return View(p);
        }
        public IActionResult Delete(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var product = _productDAL.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            _productDAL.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}