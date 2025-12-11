using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

public class AccountController : Controller
{
    string connectionString = "Data Source=DESKTOP-P8BP5TA\\SQLEXPRESS;Initial Catalog=Net107_Lab8_Bai02;Integrated Security=True;Trust Server Certificate=True";

    public IActionResult Register()
    {
        return View();
    }

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

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        bool isValidUser = false;

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

        if (isValidUser)
        {
            HttpContext.Session.SetString("UserEmail", email); //
            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Sai email hoặc mật khẩu";
        return View();
    }

    public IActionResult Dashboard()
    {
        string userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login");
        }

        ViewBag.UserEmail = userEmail;
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}