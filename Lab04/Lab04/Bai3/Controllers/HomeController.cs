using Bai3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            // Tạo danh sách Employee
            List<Employee> emp = new List<Employee>()
            {
                new Employee
                {
                    EmployeeId = 1,
                    EmployeeName = "FpolyHN",
                    Address = "Tòa nhà FPT Polytechnic, Phố Trịnh Văn Bô, Nam Từ Liêm, Hà Nội",
                    Phone = "098 172 58 36"
                },
                new Employee
                {
                    EmployeeId = 2,
                    EmployeeName = "FpolyHCM",
                    Address = "391A Nam Kỳ Khởi Nghĩa, Phường 8, Quận 3, Hồ Chí Minh",
                    Phone = "028 3526 8799"
                },
                new Employee
                {
                    EmployeeId = 3,
                    EmployeeName = "FpolyDN",
                    Address = "137 Nguyễn Thị Thập, Thanh Khê Tây, Liên Chiểu, Đà Nẵng",
                    Phone = "098 172 58 36"
                },
                new Employee
                {
                    EmployeeId = 4,
                    EmployeeName = "FpolyTN",
                    Address = "số 27, Tòa nhà VIB, đường Nguyễn Tất Thành, TP. Buôn Mê Thuột, Đắk Lắk",
                    Phone = "0262 3555 678"
                },
                new Employee
                {
                    EmployeeId = 5,
                    EmployeeName = "FpolyCT",
                    Address = "288 Đường Nguyễn Văn Linh, Hưng Lợi, Ninh Kiều, Cần Thơ",
                    Phone = "0292 7300 468"
                }
            };

            // Truyền dữ liệu sang View bằng ViewBag
            ViewBag.employee = emp;
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
