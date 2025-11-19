using Microsoft.AspNetCore.Mvc;

namespace ASM.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
