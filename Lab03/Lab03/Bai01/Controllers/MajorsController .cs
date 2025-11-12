using System.Diagnostics;
using Bai01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai01.Controllers
{
    public class MajorsController : Controller
    {
        // method hiển thị danh sách chuyên ngành
        public IActionResult Index()
        {
            var data = Major.GetMajors(); // gọi method trong Model
            return View(data);            // truyền sang View bằng @model
        }
    }
}
