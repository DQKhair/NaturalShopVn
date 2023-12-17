using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NautralShop.Areas.Admin.Controllers
{
    [Authorize]
	[Authorize(Policy = "AdminAndEmployee")]
    [Area("admin")]
	public class Statistic : Controller
    {
        private readonly NaturalShopContext _context;

        public Statistic(NaturalShopContext context) 
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetOrdersByMonth(int year)
        {
            var ordersInEachMonth = _context.Orders
                .Where(order => order.OrderDate.Year == year)
                .GroupBy(order => order.OrderDate.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    NumberOfOrders = group.Count()
                })
                .OrderBy(result => result.Month)
                .ToList();

            return Json(ordersInEachMonth);
        }

        [HttpGet]
        public IActionResult GetOrdersByPercent(int year)
        {
            var count_StatusSuccess = _context.Orders.Count(o => o.StatusOrderId == 5 && o.OrderDate.Year == year);
            var count_StatusCancel = _context.Orders.Count(order => order.StatusOrderId == 6 && order.OrderDate.Year == year);
            double totalOrders = count_StatusSuccess + count_StatusCancel;

            double percentSuccess = totalOrders != 0 ? (double)count_StatusSuccess / totalOrders * 100 : 0;
            double percentCancel = totalOrders != 0 ? (double)count_StatusCancel / totalOrders * 100 : 0;

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            };

            var result = new JsonResult(new { percentSuccess = percentSuccess, percentCancel = percentCancel }, options);
            return result;
        }
    }
}
