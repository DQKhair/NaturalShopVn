using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionsController : Controller
    {
        private readonly NaturalShopContext _context;

        public PromotionsController(NaturalShopContext context) {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var _product = await _context.Products.Where(p => p.ProductValuePromotion > 0).ToListAsync();
            return View(_product);
        }

        public  IActionResult CreatePromotion()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.ProductValuePromotion == 0), "ProductId", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromotion(Product product)
        {
            var id = product.ProductId;
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
                
            if (_product == null)
            {
                return NotFound();
            }
            else
            {
                _product.ProductValuePromotion = product.ProductValuePromotion;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }    
        }

        public async Task<IActionResult> EditPromotion(string? id)
        {
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if(_product == null)
            {
                return NotFound();
            }
            return View(_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditPromotion")]
        public async Task<IActionResult> Edit(string id, Product product)
        {
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            try
            {
                if(_product == null)
                {
                    return NotFound();
                }    
                _product.ProductValuePromotion = product.ProductValuePromotion;
                await _context.SaveChangesAsync();
            }catch
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePromotion(string? id)
        {
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
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
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            try
            {
                if (_product == null)
                {
                    return NotFound();
                }
                _product.ProductValuePromotion = 0;
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
