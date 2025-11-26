using System.Diagnostics;
using Bai01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
