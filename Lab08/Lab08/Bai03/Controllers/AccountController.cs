using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // Để dùng CookieOptions

public class AccountController : Controller
{
    string connectionString = "Data Source=DESKTOP-P8BP5TA\\SQLEXPRESS;Initial Catalog=Net107_Lab8_Bai03;Integrated Security=True;Trust Server Certificate=True";

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Authenticate(string username, string password)
    {
        bool isValid = false;

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

        if (isValid)
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true
            };

            Response.Cookies.Append("username", username, options);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
        return View("Login");
    }
}