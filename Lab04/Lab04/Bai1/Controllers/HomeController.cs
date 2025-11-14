using Bai1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult London()
        {
            ViewBag.Title = "London";
            return View();
        }
        public ActionResult Paris()
        {
            ViewBag.Title = "Paris";
            return View();
        }
        public ActionResult Tokyo()
        {
            ViewBag.Title = "Tokyo";
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
