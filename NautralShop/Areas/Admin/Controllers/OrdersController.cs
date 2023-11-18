using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly NaturalShopContext _context;

        public OrdersController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var _order = await _context.Orders.ToListAsync();
            return View(_order);
        }
    }
}
