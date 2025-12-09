using Microsoft.AspNetCore.Mvc;

namespace ASM_NET107.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Product()
        {
            ViewBag.Title = "Product";
            return View();
        }
    }
}
