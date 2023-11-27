using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;

namespace NaturalShop.Controllers
{
    public class Promotions : Controller
    {
        private readonly NaturalShopContext _context;

        public Promotions(NaturalShopContext context) {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var _productPromotion = await _context.GetListProductWithPromotion();
            if(_productPromotion == null)
            {
                return BadRequest();
            }
            return View(_productPromotion);
        }
    }
}
