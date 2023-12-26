using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class CustomersController : Controller
	{
		private readonly NaturalShopContext _context;

		public CustomersController(NaturalShopContext context) 
		{
			this._context = context;
		}

		public async Task<IActionResult> Index()
		{
			var _customers = await _context.GetListCustomers();
			return View(_customers);
		}
		public async Task<IActionResult> DetailCustomer(string? id)
		{
			if(id == null)
			{
				return NotFound();
			}
			var _customer = await _context.GetCustomerById(id);
			if( _customer == null )
			{
				return NotFound();
			}
			return View(_customer);
		}

        [HttpPut]
        public async Task<IActionResult> OffAccountCustomer(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _customer = await _context.GetCustomerById(id);
            if (_customer == null)
            {
                return BadRequest();
            }
            await _context.OffAccountCustomer(id);
            TempData["SuccessMessage"] = "Tài khoản đã ngừng hoạt động";
            return Json(_customer);
        }

        [HttpPut]
        public async Task<IActionResult> OnAccountCustomer(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _customer = await _context.GetCustomerById(id);
            if (_customer == null)
            {
                return BadRequest();
            }
            await _context.OnAccountCustomer(id);
            TempData["SuccessMessage"] = "Tài khoản đã hoạt động";
            return Json(_customer);
        }
    }
}
