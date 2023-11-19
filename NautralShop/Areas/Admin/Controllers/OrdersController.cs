using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
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
            var _order = await _context.GetListOrders();
            return View(_order);
        }

        public async Task<IActionResult> OrderDetail(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _orderDetail = await _context.GetDetailOrder(id);
            if (_orderDetail == null)
            {
                return NotFound();
            }
            ViewBag.statusOrderId = _orderDetail.FirstOrDefault()?.StatusOrderId.ToString();
            return View(_orderDetail);
        }

        [HttpPut]
        public async Task<IActionResult> ConfirmOrderById(string id)
        { 
            if (id == null)
            {
                return NotFound();
            }
            var _order = await _context.GetOrdersById(id);
            if (_order == null)
            {
                return BadRequest();
            }
            await _context.ConfirmOrder(id);
            return Json(_order);
        }

        [HttpPut]
        public async Task<IActionResult> CancelOrder(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _order = await _context.GetOrdersById(id);
            if (_order == null)
            {
                return BadRequest();
            }
            await _context.CancelOrder(id);
            return Json(_order);
        }
    }
}
