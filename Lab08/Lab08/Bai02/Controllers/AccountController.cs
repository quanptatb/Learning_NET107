using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // Cần cài gói NuGet: Microsoft.Data.SqlClient
using Microsoft.AspNetCore.Http; // Để dùng Session

public class AccountController : Controller
{
    // Chuỗi kết nối (Nên lấy từ appsettings.json như Slide 2 & 7)
    string connectionString = "Data Source=DESKTOP-P8BP5TA\\SQLEXPRESS;Initial Catalog=Net107_Lab8_Bai02;Integrated Security=True;Trust Server Certificate=True";

    // GET: Hiển thị form đăng ký
    public IActionResult Register()
    {
        return View();
    }

    // POST: Xử lý đăng ký - Lưu xuống CSDL [Dựa trên Slide 7 - Trang 22]
    [HttpPost]
    public IActionResult Register(string email, string password)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Câu lệnh Insert
            string query = "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Login");
    }

    // GET: Hiển thị form đăng nhập
    public IActionResult Login()
    {
        return View();
    }

    // POST: Xử lý đăng nhập - Kiểm tra DB và Lưu Session [Dựa trên Slide 8 - Trang 9]
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        bool isValidUser = false;

        // 1. Kiểm tra thông tin trong CSDL
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            int count = (int)cmd.ExecuteScalar();
            isValidUser = (count > 0);
        }

        // 2. Nếu đúng -> Lưu Session
        if (isValidUser)
        {
            // Lưu email vào Session với key là "UserEmail"
            HttpContext.Session.SetString("UserEmail", email); //
            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Sai email hoặc mật khẩu";
        return View();
    }

    // Trang Dashboard: Hiển thị thông tin từ Session
    public IActionResult Dashboard()
    {
        // Đọc Session
        string userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login"); // Chưa đăng nhập thì đá về trang Login
        }

        ViewBag.UserEmail = userEmail;
        return View();
    }

    // Đăng xuất: Xóa Session
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Hoặc Remove("UserEmail")
        return RedirectToAction("Login");
    }
}