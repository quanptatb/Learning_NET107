using Bai02.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.WebSockets;

namespace Bai02.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var data = MajorModel.GetMajors();
            return View(data);
        }
    }
}
