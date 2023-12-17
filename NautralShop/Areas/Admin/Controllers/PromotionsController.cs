using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class PromotionsController : Controller
    {
        private readonly NaturalShopContext _context;

        public PromotionsController(NaturalShopContext context) {
            this._context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var _product = await _context.GetListPromotions(search??"");
            return View(_product);
        }

        public async Task<IActionResult> CreatePromotion()
        {
            ViewData["ProductId"] = new SelectList( await _context.GetPromotionNotValue(), "ProductId", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromotion(Product product)
        {
            string id = product.ProductId;
            var _product = await _context.GetProductById(id);
                
            if (_product == null || product.ProductValuePromotion == null)
            {
                return NotFound();
            }
            else
            {
                await _context.AddAndEditPromotion(id,Convert.ToDouble(product.ProductValuePromotion)!);
                return RedirectToAction(nameof(Index));
            }    
        }

        public async Task<IActionResult> EditPromotion(string? id)
        {
            if(id == null)
            {
                return BadRequest();
            }else
            {
                var _product = await _context.GetProductById(id);
                if(_product == null)
                {
                    return NotFound();
                }
                return View(_product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditPromotion")]
        public async Task<IActionResult> Edit(string id, Product product)
        {
            var _product = await _context.GetProductById(id);
            try
            {
                if(_product == null || product.ProductValuePromotion == null)
                {
                    return NotFound();
                }else
                {
                    await _context.AddAndEditPromotion(id,Convert.ToDouble(product.ProductValuePromotion)!);
                }    
            }catch
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePromotion(string? id)
        {
            var _product = await _context.GetProductById(id!);
            if (_product == null)
            {
                return NotFound();
            }
            return View(_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotion(string id, Product product)
        {
            var _product = await _context.GetProductById(id);
            try
            {
                if (_product == null)
                {
                    return NotFound();
                }
                await _context.DeletePromotion(id);
            }
            catch
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
