using ASM_NET107_TB01758.DAL;
using ASM_NET107_TB01758.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ASM_NET107_TB01758.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _db;
        public AdminController(IConfiguration conf) { _db = new DatabaseHelper(conf); }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "0") return RedirectToAction("Login", "Account");
            // List nhân viên
            return View(_db.GetRecords("SELECT * FROM Users WHERE Role=1"));
        }

        [HttpPost]
        public IActionResult CreateStaff(string username, string password, string fullname)
        {
            _db.ExecuteNonQuery("INSERT INTO Users (Username, Password, FullName, Role) VALUES (@u, @p, @fn, 1)",
                new SqlParameter("@u", username), new SqlParameter("@p", password), new SqlParameter("@fn", fullname));
            return RedirectToAction("Index");
        }

        public IActionResult DeleteStaff(int id)
        {
            _db.ExecuteNonQuery("DELETE FROM Users WHERE Id=@id AND Role=1", new SqlParameter("@id", id));
            return RedirectToAction("Index");
        }

        public IActionResult Statistics()
        {
            if (HttpContext.Session.GetString("Role") != "0") return RedirectToAction("Login", "Account");

            // Thống kê: Tổng doanh thu từ đơn hàng đã duyệt (Status=2)
            string sql = @"SELECT SUM(d.Price * d.Quantity) 
                       FROM Carts c 
                       JOIN Cart_Detail d ON c.Id = d.CartId 
                       WHERE c.Status = 2";
            object result = _db.ExecuteScalar(sql);
            ViewBag.Revenue = result != DBNull.Value ? result : 0;
            return View();
        }
        // GET: Form sửa nhân viên
        public IActionResult EditStaff(int id)
        {
            if (HttpContext.Session.GetString("Role") != "0") return RedirectToAction("Login", "Account");

            var dt = _db.GetRecords("SELECT * FROM Users WHERE Id=@id AND Role=1", new SqlParameter("@id", id));
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

        // POST: Lưu nhân viên
        [HttpPost]
        public IActionResult EditStaff(AppUser user)
        {
            if (HttpContext.Session.GetString("Role") != "0") return Unauthorized();

            string sql = "UPDATE Users SET FullName=@fn, Email=@e WHERE Id=@id AND Role=1";
            _db.ExecuteNonQuery(sql,
                new SqlParameter("@fn", user.FullName),
                new SqlParameter("@e", user.Email),
                new SqlParameter("@id", user.Id));

            return RedirectToAction("Index");
        }
    }
}
