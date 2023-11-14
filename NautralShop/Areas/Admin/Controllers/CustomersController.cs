using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
	public class CustomersController : Controller
	{
		private readonly NaturalShopContext _context;

		public CustomersController(NaturalShopContext context) 
		{
			this._context = context;
		}

		public IActionResult Index()
		{

			return View();
		}
	}
}
