using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
using NautralShop.Models;
using System.Security.Claims;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class OrdersController : Controller
    {
        private readonly NaturalShopContext _context;

        public OrdersController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public async Task<IActionResult> Index(int? page, string? search)
        {
                var _order = await _context.Orders.Include(m => m.PaymentMethod).Include(s => s.StatusOrder).OrderByDescending(o => o.OrderDate).Where(s => s.OrderCustomerName.Contains(search??"")).ToListAsync();
                var pageSize = 8;
                var pageNumber = page ?? 1;
                var pageList = _order.ToPagedList(pageNumber, pageSize);
                return View(pageList);
           
        }

        public async Task<IActionResult> OrderDetail(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _orderDetail = await _context.OrderDetails.Where(o => o.OrderId == id).Include(o => o.Order).Include(s => s.Order!.StatusOrder).Include(p=> p.Product).ToListAsync();
            var _orderDetailInfomation = await _context.OrderDetails.Include(o => o.Order).Include(s => s.Order!.StatusOrder).Include(p => p.Product).FirstOrDefaultAsync(o => o.OrderId == id);
            if (_orderDetail == null || _orderDetailInfomation == null)
            {
                return NotFound();
            }
            ViewBag.OrderDetailInfomation = _orderDetailInfomation;
            ViewBag.statusOrderId = _orderDetail.FirstOrDefault()?.Order!.StatusOrderId.ToString();
            return View(_orderDetail);
        }

        [HttpPut("/admin/orders/ConfirmOrderById/orderId={orderId}")]
        public async Task<IActionResult> ConfirmOrderById(string orderId)
        { 
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return BadRequest();
            }    
            if (orderId == null)
            {
                return NotFound();
            }
            var _order = await _context.GetOrdersById(orderId);
            if (_order == null)
            {
                return BadRequest();
            }
            string employeeId = claimsIdentity.FindFirst("EmployeeId")!.Value;
            await _context.ConfirmOrder(orderId,employeeId);
            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn thành công";
            return Json(_order);
        }

        [HttpPut("/admin/orders/CancelOrder/orderId={orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return BadRequest();
            }
            if (orderId == null)
            {
                return NotFound();
            }
            var _order = await _context.GetOrdersById(orderId);
            if (_order == null)
            {
                return BadRequest();
            }
            string employeeId = claimsIdentity.FindFirst("EmployeeId")!.Value;
            await _context.CancelOrder(orderId,employeeId);
            TempData["SuccessMessage"] = "Hủy đơn thành công";
            return Json(_order);
        }
    }
}
