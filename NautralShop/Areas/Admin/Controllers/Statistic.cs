using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using static NuGet.Packaging.PackagingConstants;

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
            var count_StatusSuccess = _context.Orders.Count(order => order.StatusOrderId == 5 && order.OrderDate.Year == year);
            var count_StatusCancel = _context.Orders.Count(order => order.StatusOrderId == 6 && order.OrderDate.Year == year);
            double percentSuccess = Convert.ToDouble(( Convert.ToDouble(count_StatusSuccess) / (Convert.ToDouble(count_StatusSuccess) + Convert.ToDouble(count_StatusCancel)))) * 100;
            double percentCancel = Convert.ToDouble((Convert.ToDouble(count_StatusCancel) / (Convert.ToDouble(count_StatusSuccess) + Convert.ToDouble(count_StatusCancel)))) * 100;

            return Json(new { percentSuccess = percentSuccess, percentCancel = percentCancel });
        }
    }
}
