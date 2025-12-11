using ASM_NET107_TB01758.DAL;
using ASM_NET107_TB01758.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ASM_NET107_TB01758.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;
        public AccountController(IConfiguration conf) { _db = new DatabaseHelper(conf); }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var dt = _db.GetRecords("SELECT * FROM Users WHERE Username=@u AND Password=@p",
                new SqlParameter("@u", username), new SqlParameter("@p", password));

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                HttpContext.Session.SetString("UserId", row["Id"].ToString());
                HttpContext.Session.SetString("Role", row["Role"].ToString());
                HttpContext.Session.SetString("FullName", row["FullName"].ToString());

                int role = Convert.ToInt32(row["Role"]);
                if (role == 0) return RedirectToAction("Index", "Admin"); // Quản lý
                if (role == 1) return RedirectToAction("Index", "Product"); // Nhân viên
                return RedirectToAction("Index", "Home"); // Khách hàng
            }
            ViewBag.Error = "Sai thông tin đăng nhập!";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(AppUser user)
        {
            // Mặc định tạo tài khoản là Khách hàng (Role = 2)
            string sql = "INSERT INTO Users (Username, Password, FullName, Email, Role) VALUES (@u, @p, @fn, @e, 2)";
            _db.ExecuteNonQuery(sql,
                new SqlParameter("@u", user.Username),
                new SqlParameter("@p", user.Password),
                new SqlParameter("@fn", user.FullName),
                new SqlParameter("@e", user.Email));
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Chức năng quản lý thông tin cá nhân (Yêu cầu Khách hàng & Nhân viên)
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");

            var dt = _db.GetRecords("SELECT * FROM Users WHERE Id=@id", new SqlParameter("@id", userId));
            if (dt.Rows.Count == 0) return NotFound();

            var row = dt.Rows[0];
            AppUser user = new AppUser
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString()
            };
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(AppUser user)
        {
            var userId = HttpContext.Session.GetString("UserId");
            string sql = "UPDATE Users SET FullName=@fn, Email=@e WHERE Id=@id";
            _db.ExecuteNonQuery(sql,
                new SqlParameter("@fn", user.FullName),
                new SqlParameter("@e", user.Email),
                new SqlParameter("@id", userId));

            // Cập nhật lại Session
            HttpContext.Session.SetString("FullName", user.FullName);
            ViewBag.Message = "Cập nhật thành công!";
            return View("Profile", user);
        }
    }
}