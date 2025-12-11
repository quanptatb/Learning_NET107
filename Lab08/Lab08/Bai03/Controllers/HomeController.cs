using Bai03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai03.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string username = Request.Cookies["username"];

            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.UserName = username;
                ViewBag.Message = "Xin chào, " + username;
            }
            else
            {
                ViewBag.Message = "Chào mừng bạn (Khách)";
            }

            return View();
        }

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
