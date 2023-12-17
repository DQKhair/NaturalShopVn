using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NaturalShop.Controllers
{
    [Authorize]
    public class InfomationsController : Controller
    {
        private readonly NaturalShopContext _context;
        public InfomationsController(NaturalShopContext context) 
        {
            this._context = context; ;
        }

        public IActionResult Index(string customerId)
        {
            if (customerId == null)
            {
                return BadRequest();
            }
            var customer = _context.Customers.Include(c=>c.AccountType).SingleOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeInfomationCustomer(Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            var _customer = await _context.Customers.SingleOrDefaultAsync(e => e.CustomerId == customer.CustomerId);
            if (_customer == null)
            {
                return BadRequest();
            }
            _customer.CustomerName = customer.CustomerName;
            _customer.CustomerPhone = customer.CustomerPhone;
            _customer.CustomerAddress = customer.CustomerAddress;
            _customer.CustomerEmail = customer.CustomerEmail;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { customerId = _customer.CustomerId });
        }

        [HttpPost("infomations/ChangePassword/customerId={customerId}&&oldPass={oldPass}&&newPass={newPass}")]
        public async Task<IActionResult> ChangePassword(string customerId, string oldPass, string newPass)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(e => e.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }
            if (BCrypt.Net.BCrypt.Verify(oldPass, customer.CustomerPassword))
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                customer.CustomerPassword = BCrypt.Net.BCrypt.HashPassword(newPass, salt);
                await _context.SaveChangesAsync();
                return Json(Ok());
            }
            else
            {
                return BadRequest("Mật khẩu cũ không đúng");
            }
        }

        public async Task<IActionResult> HistoryOrder(string customerId)
        {
            if(customerId == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.Include(s => s.StatusOrder).Where(c => c.CustomerId == customerId).ToListAsync();
            if (order == null)
            {
                return NotFound();
            }    
            return View(order);
        }

        [HttpGet("/HistoryOrderDetail/orderId={orderId}")]
        public async Task<IActionResult> HistoryOrderDetail(string orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }
            var orderDetail = await _context.OrderDetails.Include(o => o.Order).ThenInclude(s => s.StatusOrder).Include(c => c.Order!.Customer).Include(p => p.Product).Where(c => c.OrderId == orderId).Select(s => new
            {
                orderDetailId = s.OrderDetailId,
                productId = s.ProductId,
                productName = s.Product!.ProductName,
                orderDetailQuantity = s.OrderDetailQuantity,
                orderDetailPrice = s.OrderDetailPrice,
                customerName = s.Order!.OrderCustomerName,
                customerPhone = s.Order.OrderCustomerPhone,
                customerAddress = s.Order.OrderCustomerAddress,
                orderStatus = s.Order!.StatusOrder!.StatusOrderName,
                orderDate = s.Order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"),
                orderId = s.OrderId
            }).ToListAsync();

            if (orderDetail == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            };

            var result = new JsonResult(orderDetail, options);
            return result;
        }
    }
}
