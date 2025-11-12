using Bai02.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bai02.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = new List<Product>
        {
            new(){Name="iPhone 11 Pro Max", 
                Capacity="64GB", 
                Image="/images/iphone1.png", 
                Price=33990000, 
                Discount=2000000, 
                Installment="1.512.000đ/tháng", 
                Rating=4.5, Reviews=439, 
                Screen="1242×2688 Pixels 6.5 in", 
                Camera="Triple 12MP Ultra Wide", 
                Battery="Lâu hơn iPhone Xs Max 5h", 
                Ram="4 GB", 
                Cpu="Apple A13 Bionic", 
                Os="iOS 13"},
            new(){Name="iPhone 11 Pro Max", 
                Capacity="256GB", 
                Image="/images/iphone2.png", 
                Price=37990000, 
                Discount=1500000, 
                Installment="1.689.000đ/tháng", 
                Rating=4.3, 
                Reviews=67, 
                Screen="1242×2688 Pixels 6.5 in", 
                Camera="Triple 12MP Ultra Wide", 
                Battery="Lâu hơn iPhone Xs Max 5h", 
                Ram="4 GB", 
                Cpu="Apple A13 Bionic", 
                Os="iOS 13"},
            new(){Name="iPhone 11 Pro Max", 
                Capacity="512GB", 
                Image="/images/iphone3.png", 
                Price=43990000, 
                Discount=2000000, 
                Installment="1.565.000đ/tháng", 
                Rating=4.7, 
                Reviews=59, 
                Screen="1242×2688 Pixels 6.5 in", 
                Camera="Triple 12MP Ultra Wide", 
                Battery="Lâu hơn iPhone Xs Max 5h", 
                Ram="4 GB", 
                Cpu="Apple A13 Bionic", 
                Os="iOS 13"},
            new(){Name="iPhone 11 Pro", 
                Capacity="64GB", 
                Image="/images/iphone4.png", 
                Price=32990000, 
                Discount=1500000, 
                Installment="1.653.000đ/tháng", 
                Rating=4.6, 
                Reviews=59, 
                Screen="1125×2436 Pixels 5.8 in", 
                Camera="Triple 12MP Ultra Wide", 
                Battery="Lâu hơn iPhone Xs 4h", 
                Ram="4 GB", 
                Cpu="Apple A13 Bionic", 
                Os="iOS 13"}
        };
            return View(products);
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
    }
}
