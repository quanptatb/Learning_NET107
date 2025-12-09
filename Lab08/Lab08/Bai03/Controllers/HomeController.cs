using Bai03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai03.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Đọc cookie "username" từ request gửi lên (Slide 8 - Trang 24)
            string username = Request.Cookies["username"];

            if (!string.IsNullOrEmpty(username))
            {
                // Nếu có cookie -> Gửi tên qua ViewBag để hiển thị lời chào cá nhân
                ViewBag.UserName = username;
                ViewBag.Message = "Xin chào, " + username;
            }
            else
            {
                // Nếu không có cookie
                ViewBag.Message = "Chào mừng bạn (Khách vãng lai)";
            }

            return View();
        }

        // Bổ sung: Action Đăng xuất để xóa Cookie (Slide 8 - Trang 25)
        public IActionResult Logout()
        {
            Response.Cookies.Delete("username");
            return RedirectToAction("Index");
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
