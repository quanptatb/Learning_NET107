using ASM_NET107_TB01758.DAL;
using ASM_NET107_TB01758.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ASM_NET107_TB01758.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseHelper _db;
        public ProductController(IConfiguration configuration) { _db = new DatabaseHelper(configuration); }

        // Chỉ nhân viên (Role=1) hoặc Admin (Role=0) mới được vào
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "1" && role != "0") return RedirectToAction("Login", "Account");

            var dt = _db.GetRecords("SELECT * FROM Products");
            // Chuyển đổi DataTable sang List<Product> để truyền qua View
            // ... (Code mapping dữ liệu tương tự slide 21)
            return View(dt);
        }
        private List<SelectListItem> GetCategories()
        {
            var dt = _db.GetRecords("SELECT * FROM Categories");
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem { Value = row["Id"].ToString(), Text = row["Name"].ToString() });
            }
            return list;
        }
        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories(); // Truyền danh mục sang View
            return View();
        }
        // Create (Thêm sản phẩm)
        [HttpPost]
        public IActionResult Create(Product model)
        {
            string sql = "INSERT INTO Products (Name, Price, Image, Color, Size, CategoryId, Description) VALUES (@n, @p, @i, @c, @s, @cat, @d)";
            var paramss = new SqlParameter[] {
                new SqlParameter("@n", model.Name),
                new SqlParameter("@p", model.Price),
                new SqlParameter("@i", model.Image ?? "no-img.jpg"), // Xử lý nếu null
                new SqlParameter("@c", model.Color ?? ""),
                new SqlParameter("@s", model.Size ?? ""),
                new SqlParameter("@cat", model.CategoryId), // <--- ĐÃ SỬA: Lấy từ Model, không hardcode số 1
                new SqlParameter("@d", model.Description ?? "")
            };
            _db.ExecuteNonQuery(sql, paramss);
            return RedirectToAction("Index");
        }
        // ... (Code cũ giữ nguyên)

        // EDIT: Hiển thị form sửa
        public IActionResult Edit(int id)
        {
            var dt = _db.GetRecords("SELECT * FROM Products WHERE Id=@id", new SqlParameter[] { new SqlParameter("@id", id) });
            if (dt.Rows.Count == 0) return NotFound();
            var row = dt.Rows[0];
            var p = new Product
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                Image = row["Image"].ToString(),
                Color = row["Color"].ToString(),
                Size = row["Size"].ToString(),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Description = row["Description"].ToString()
            };
            ViewBag.Categories = GetCategories(); // Truyền danh mục sang View
            return View(p);
        }

        // UPDATE: Lưu thay đổi
        // UPDATE: Lưu thay đổi (Đã bổ sung Description, CategoryId, Image)
        [HttpPost]
        public IActionResult Edit(Product model)
        {
            // Câu lệnh SQL cập nhật đầy đủ các trường
            string sql = @"UPDATE Products 
                   SET Name=@n, Price=@p, Color=@c, Size=@s, 
                       Description=@d, CategoryId=@cat, Image=@i 
                   WHERE Id=@id";

            var parameters = new SqlParameter[] {
                new SqlParameter("@n", model.Name),
                new SqlParameter("@p", model.Price),
                new SqlParameter("@c", model.Color ?? ""),
                new SqlParameter("@s", model.Size ?? ""),
                new SqlParameter("@d", model.Description ?? ""),
                new SqlParameter("@cat", model.CategoryId), // Cập nhật danh mục
                new SqlParameter("@i", model.Image ?? ""),  // Cập nhật hình ảnh (nếu có logic upload ảnh, cần xử lý trước dòng này)
                new SqlParameter("@id", model.Id)
            };

            _db.ExecuteNonQuery(sql, parameters);
            return RedirectToAction("Index");
        }

        // DELETE: Xóa sản phẩm
        public IActionResult Delete(int id)
        {
            _db.ExecuteNonQuery("DELETE FROM Products WHERE Id=@id", new SqlParameter[] { new SqlParameter("@id", id) });
            return RedirectToAction("Index");
        }

        // DUYỆT ĐƠN HÀNG (Staff only)
        // Trong Controllers/ProductController.cs

        public IActionResult ManageOrders()
        {
            // Kiểm tra quyền nhân viên
            var role = HttpContext.Session.GetString("Role");
            if (role != "1" && role != "0") return RedirectToAction("Login", "Account");

            // Câu lệnh SQL JOIN 3 bảng để lấy: Mã đơn, Tên khách, Ngày đặt, Tổng tiền
            string sql = @"
        SELECT 
            c.Id, 
            u.FullName, 
            c.CreatedDate, 
            SUM(d.Price * d.Quantity) as Total
        FROM Carts c
        JOIN Users u ON c.UserId = u.Id
        JOIN Cart_Detail d ON c.Id = d.CartId
        WHERE c.Status = 1
        GROUP BY c.Id, u.FullName, c.CreatedDate";

            var dt = _db.GetRecords(sql);
            return View(dt);
        }

        public IActionResult ApproveOrder(int id)
        {
            _db.ExecuteNonQuery("UPDATE Carts SET Status = 2 WHERE Id=@id", new SqlParameter[] { new SqlParameter("@id", id) });
            return RedirectToAction("ManageOrders");
        }

    }
}
