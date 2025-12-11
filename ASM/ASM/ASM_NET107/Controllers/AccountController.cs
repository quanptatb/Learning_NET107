using ASM_NET107.DAL;
using ASM_NET107.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NET107.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeeDAL _empDAL;
        private readonly CustomerDAL _cusDAL;

        public AccountController(EmployeeDAL empDAL, CustomerDAL cusDAL)
        {
            _empDAL = empDAL;
            _cusDAL = cusDAL;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password, string type)
        {
            if (type == "Employee")
            {
                var emp = _empDAL.CheckLogin(username, password);
                if (emp != null)
                {
                    // QUAN TRỌNG: Lưu đúng Role của nhân viên lấy từ DB
                    HttpContext.Session.SetString("UserRole", emp.Role);
                    HttpContext.Session.SetString("Username", emp.FullName);

                    // Điều hướng dựa trên Role nếu cần, hoặc về trang chủ Admin chung
                    return RedirectToAction("Index", "Product");
                }
            }
            else // Khách hàng đăng nhập (dùng Phone làm username)
            {
                var cus = _cusDAL.CheckLogin(username, password);
                if (cus != null)
                {
                    // Lưu Session cho Khách hàng
                    HttpContext.Session.SetString("UserRole", "Customer");
                    HttpContext.Session.SetString("Username", cus.CustomerName);
                    return RedirectToAction("Index", "Home"); // Trang chủ bán hàng
                }
            }
            ViewBag.Error = "Sai thông tin đăng nhập";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Customers customer)
        {
            _cusDAL.Register(customer);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session
            return RedirectToAction("Login");
        }
        public IActionResult Profile()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Customer") return RedirectToAction("Login");

            // Giả sử bạn đã lưu Phone vào Session khi Login (bạn nên sửa Login để lưu Phone)
            var phone = HttpContext.Session.GetString("Phone");
            var cus = _cusDAL.GetCustomerByPhone(phone);
            return View(cus);
        }

        [HttpPost]
        public IActionResult Profile(Customers c)
        {
            _cusDAL.UpdateCustomer(c);
            ViewBag.Message = "Cập nhật thành công!";
            return View(c);
        }
    }
}