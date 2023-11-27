using Microsoft.AspNetCore.Mvc;

namespace NaturalShop.Controllers
{
	public class News : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult DetailNews(int? id) 
		{ 
			if (id == null)
			{
				return NotFound();
			}

			ViewData["NewsId"] = id;

			return View();
		}
	}
}
