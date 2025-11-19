using System.Diagnostics;
using ASM.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Employee()
        {
            ViewBag.Title = "Employee";
            return View();
        }
        public ActionResult Order()
        {
            ViewBag.Title = "Order";
            return View();
        }
        public ActionResult Product()
        {
            ViewBag.Title = "Product";
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
