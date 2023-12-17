using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using static NuGet.Packaging.PackagingConstants;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class PageAdminController : Controller
    {
        private readonly NaturalShopContext _context;

        public PageAdminController(NaturalShopContext context) 
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/admin/homePage/GetStatisticOrderCountMonthly")]
        public async Task<IActionResult> GetStatisticOrderCountMonthly()
        {
            DateTime current = DateTime.Now;
            var month = current.Month;
            var year = current.Year;
            //handle count order monthly
            var orderCount = await _context.Orders.CountAsync(s => s.OrderDate.Month == month && s.OrderDate.Year == year);


            //handle money monthly
            var orderMoney = await _context.Orders.Where(s => s.OrderDate.Month == month && s.OrderDate.Year == year).SumAsync(s => s.OrderTotalPrice);
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            };

            var result = new JsonResult(new { orderCount = orderCount, orderMoney = orderMoney.ToString("n0") });
            return result;
        }

    }
}
