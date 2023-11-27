using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;

namespace NaturalShop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly NaturalShopContext _context;

        public CategoriesController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _product = await _context.GetListProductWithCateId(id??0);
            var _category = await _context.GetCategoryById(id);
            if (_product == null)
            {
                return BadRequest();
            }
            if (_category == null)
            {
                return BadRequest();
            }
            ViewData["CateById"] = _category;
            return View(_product);
        }
    }
}
