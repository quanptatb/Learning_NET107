using ASM_NET107_TB01758.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ASM_NET107_TB01758.Controllers
{
    public class CartController : Controller
    {
        private readonly DatabaseHelper _db;
        public CartController(IConfiguration conf) { _db = new DatabaseHelper(conf); }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            // Lấy giỏ hàng đang active (Status = 0)
            string sql = @"SELECT d.Id, p.Name, p.Image, d.Price, d.Quantity, (d.Price * d.Quantity) as Total
                       FROM Carts c
                       JOIN Cart_Detail d ON c.Id = d.CartId
                       JOIN Products p ON d.ProductId = p.Id
                       WHERE c.UserId = @uid AND c.Status = 0";
            var dt = _db.GetRecords(sql, new SqlParameter("@uid", userId));
            return View(dt);
        }

        public IActionResult AddToCart(int id) // id = ProductId
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            // 1. Kiểm tra User đã có giỏ hàng (Status=0) chưa
            var cartDt = _db.GetRecords("SELECT Id FROM Carts WHERE UserId=@uid AND Status=0", new SqlParameter("@uid", userId));
            int cartId;

            if (cartDt.Rows.Count == 0)
            {
                // Tạo giỏ mới
                _db.ExecuteNonQuery("INSERT INTO Carts (UserId, Status) VALUES (@uid, 0)", new SqlParameter("@uid", userId));
                cartId = (int)_db.ExecuteScalar("SELECT TOP 1 Id FROM Carts WHERE UserId=@uid AND Status=0 ORDER BY Id DESC", new SqlParameter("@uid", userId));
            }
            else
            {
                cartId = (int)cartDt.Rows[0]["Id"];
            }

            // 2. Lấy giá sản phẩm
            decimal price = (decimal)_db.ExecuteScalar("SELECT Price FROM Products WHERE Id=@id", new SqlParameter("@id", id));

            // 3. Kiểm tra sản phẩm đã có trong giỏ chưa
            var detailDt = _db.GetRecords("SELECT Id, Quantity FROM Cart_Detail WHERE CartId=@cid AND ProductId=@pid",
                new SqlParameter("@cid", cartId), new SqlParameter("@pid", id));

            if (detailDt.Rows.Count > 0)
            {
                // Đã có -> Tăng số lượng
                int newQty = (int)detailDt.Rows[0]["Quantity"] + 1;
                _db.ExecuteNonQuery("UPDATE Cart_Detail SET Quantity=@q WHERE Id=@did",
                    new SqlParameter("@q", newQty), new SqlParameter("@did", detailDt.Rows[0]["Id"]));
            }
            else
            {
                // Chưa có -> Thêm mới
                _db.ExecuteNonQuery("INSERT INTO Cart_Detail (CartId, ProductId, Quantity, Price) VALUES (@cid, @pid, 1, @p)",
                    new SqlParameter("@cid", cartId), new SqlParameter("@pid", id), new SqlParameter("@p", price));
            }

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id) // id = Cart_Detail Id
        {
            _db.ExecuteNonQuery("DELETE FROM Cart_Detail WHERE Id=@id", new SqlParameter("@id", id));
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            // Chuyển trạng thái giỏ hàng từ 0 (đang mua) -> 1 (chờ duyệt)
            _db.ExecuteNonQuery("UPDATE Carts SET Status = 1, CreatedDate = GETDATE() WHERE UserId=@uid AND Status=0",
                new SqlParameter("@uid", userId));
            return RedirectToAction("Index", "Home"); // Quay về trang chủ
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity) // id là Cart_Detail Id
        {
            if (quantity < 1) return RedirectToAction("Remove", new { id = id });

            _db.ExecuteNonQuery("UPDATE Cart_Detail SET Quantity=@q WHERE Id=@id",
                new SqlParameter("@q", quantity),
                new SqlParameter("@id", id));

            return RedirectToAction("Index");
        }
    }
}
