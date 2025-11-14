using Bai2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // String data
            ViewData["Title"] = "Student Details Page";
            ViewData["Header"] = "Student Details";

            // Khởi tạo dữ liệu cho Student
            StudentModel student = new StudentModel()
            {
                StudentId = 1,
                Name = "Fpoly",
                Branch = "Hcm",
                Section = "Nam Ky Khoi Nghia"
            };

            // Gán Student cho ViewData
            ViewData["StudentModel"] = student;

            // Trả về View Index
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
