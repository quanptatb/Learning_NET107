using Microsoft.AspNetCore.Mvc;

namespace ASM.Controllers
{
    public class AdminController : Controller
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
    }
}
