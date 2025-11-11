using Bai02.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai02.Controllers
{
    public class Iphone11Controller : Controller
    {
        public IActionResult Index()
        {
            var data = IphoneModel.GetIphones();
            return View(data);
        }
    }
}
