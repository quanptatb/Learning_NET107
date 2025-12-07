using Microsoft.AspNetCore.Mvc;

namespace ASM_NET107.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Order()
        {
            ViewBag.Title = "Order";
            return View();
        }
    }
}
