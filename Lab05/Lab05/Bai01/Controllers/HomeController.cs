using System.Diagnostics;
using Bai01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var data = MajorModel.GetMajors();
            ViewData["Title"] = "Công nghệ thông tin";
            return View(data);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
