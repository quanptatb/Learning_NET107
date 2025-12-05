using Microsoft.AspNetCore.Mvc;

namespace ASM.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult blank()
        {
            return View();
        }
        public IActionResult checkout()
        {
            return View();
        }
        public IActionResult product()
        {
            return View();
        }
        public IActionResult store()
        {
            return View();
        }
    }
}
