using ASM_NET107_TB01758.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using ASM_NET107_TB01758.DAL;

namespace ASM_NET107_TB01758.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _db;

        // --- PHẦN BỔ SUNG BẮT BUỘC ---
        public HomeController(IConfiguration conf)
        {
            _db = new DatabaseHelper(conf);
        }
        // -----------------------------

        public IActionResult Index(string searchString, decimal? minPrice)
        {
            string sql = "SELECT * FROM Products WHERE 1=1";
            List<SqlParameter> pList = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchString))
            {
                sql += " AND Name LIKE @search";
                pList.Add(new SqlParameter("@search", "%" + searchString + "%"));
            }
            if (minPrice.HasValue)
            {
                sql += " AND Price >= @price";
                pList.Add(new SqlParameter("@price", minPrice));
            }

            // _db bây giờ đã được khởi tạo, không còn bị null
            var dt = _db.GetRecords(sql, pList.ToArray());
            return View(dt);
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

        // Bổ sung Action Details cho chức năng xem chi tiết sản phẩm (như đã hướng dẫn ở câu trả lời trước)
        public IActionResult Details(int id)
        {
            string sql = "SELECT * FROM Products WHERE Id = @id";
            var dt = _db.GetRecords(sql, new SqlParameter("@id", id));

            if (dt.Rows.Count == 0) return NotFound();

            var row = dt.Rows[0];
            // Bạn có thể truyền thẳng DataRow hoặc mapping sang Product model
            // Để đơn giản và nhanh chóng, truyền DataRow sang View cũng được
            return View(row);
        }
    }
}