using ASM_NET107.DAL;
using ASM_NET107.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // Để serialize list cart vào session

namespace ASM_NET107.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAL _productDAL;
        public CartController(ProductDAL productDAL) => _productDAL = productDAL;

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart != null)
            {
                return JsonSerializer.Deserialize<List<CartItem>>(sessionCart);
            }
            return new List<CartItem>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        public IActionResult AddToCart(string id)
        {
            var product = _productDAL.GetProductById(id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductID == id);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    ImageURL = product.ImageURL,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(string id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // Bạn có thể thêm Action UpdateQuantity tại đây...
        public IActionResult UpdateQuantity(string id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductID == id);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
    }
}