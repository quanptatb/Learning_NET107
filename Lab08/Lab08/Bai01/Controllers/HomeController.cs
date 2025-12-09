using System.Diagnostics;
using Bai01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            HttpContext.Session.SetString("name", "Phan Viet The");
            HttpContext.Session.SetString("email", "thepv@uit.edu.vn.com");
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Name = HttpContext.Session.GetString("name");
            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewData["Message"] = "Your about page, please refesh page after one minute";
            ViewData["Title"] = "Demo session login";
            return View();
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
