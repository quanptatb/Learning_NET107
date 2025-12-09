using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // Để dùng CookieOptions

public class AccountController : Controller
{
    // Chuỗi kết nối (Thay đổi theo máy của bạn - Slide 7 Trang 8)
    string connectionString = "Data Source=DESKTOP-P8BP5TA\\SQLEXPRESS;Initial Catalog=Net107_Lab8_Bai03;Integrated Security=True;Trust Server Certificate=True";

    // 1. Action Login (GET): Hiển thị form
    public IActionResult Login()
    {
        return View();
    }

    // 2. Action Authenticate (POST): Xử lý đăng nhập
    [HttpPost]
    public IActionResult Authenticate(string username, string password)
    {
        bool isValid = false;

        // --- Check Database (Kiến thức Slide 7) ---
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @u AND Password = @p";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            int count = (int)cmd.ExecuteScalar();
            isValid = (count > 0);
        }

        // --- Xử lý Cookie (Kiến thức Slide 8) ---
        if (isValid)
        {
            // Tạo CookieOptions để cấu hình (Slide 8 - Trang 23)
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1), // Cookie sống 1 ngày
                HttpOnly = true,
                IsEssential = true
            };

            // Tạo Cookie tên "username" lưu giá trị tên người dùng
            Response.Cookies.Append("username", username, options);

            // Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
        return View("Login");
    }
}