using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class PageAdminController : Controller
    {
        public IActionResult Index()
        {
            //// Lấy thông tin xác thực từ HttpContext
            //var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            //// Kiểm tra xem có thông tin xác thực không
            //if (claimsIdentity != null)
            //{
            //    // Lấy các claim từ Identity
            //    var userName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            //    var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            //    // Sử dụng thông tin claim theo nhu cầu của bạn
            //}

            return View();

        }

    }
}
