using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NautralShop.Controllers
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
            // Lấy thông tin xác thực từ HttpContext
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            // Kiểm tra xem có thông tin xác thực không
            if (claimsIdentity != null)
            {
                // Lấy các claim từ Identity
                var userName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

                // Sử dụng thông tin claim theo nhu cầu của bạn
                ViewBag.UserName = userName;
                ViewBag.Role = role;
            }
            return View();
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