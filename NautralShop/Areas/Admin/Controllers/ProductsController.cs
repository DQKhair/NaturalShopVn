using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
using NautralShop.Models;
using System.Net.WebSockets;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly NaturalShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(NaturalShopContext context, IWebHostEnvironment _webHostEnvironment) 
        {
            this._context = context;
            this._webHostEnvironment = _webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var _products = await _context.Products.Include(c=>c.Category).Where(p => p.ProductStatus == true).ToListAsync();
            return View(_products);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> AddProduct([FromForm]ProductsVM productsVM)
        {
            if(productsVM == null)
            {
                return BadRequest();
            }    
            var _product = new Product();
            _product.ProductId = Guid.NewGuid().ToString();
            _product.ProductName = productsVM.ProductName;
            _product.ProductPrice = productsVM.ProductPrice;
            _product.ProductValuePromotion = productsVM.ProductValuePromotion;
            _product.CategoryId = productsVM.CategoryId;
            _product.ProductIngredient = productsVM.ProductIngredient;
            _product.ProductUseful = productsVM.ProductUseful;
            _product.ProductUserManual = productsVM.ProductUserManual;
            _product.ProductDescription = productsVM.ProductDescription;
            _product.ProductStatus = true;
            _product.ProductDetailDescription = productsVM.ProductDetailDescription;
            if(productsVM.IFormFileImage != null && productsVM.IFormFileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                try
                {
                    using( var stream = new FileStream(filePath,FileMode.Create))
                    {
                        await productsVM.IFormFileImage.CopyToAsync(stream);
                    }
                    _product.ProductImage = "/images/" + uniqueFileName;

                }catch
                {
                    return BadRequest("Lỗi khi lưu trữ hình ảnh: ");
                }
            }else
            {
                _product.ProductImage = "";
            }
            _context.Add(_product);
            await _context.SaveChangesAsync();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", productsVM.CategoryId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditProduct(string? id)
        {
            if(String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if (_product == null)
            {
                return NotFound();
            }
            ProductsVM productsVM = new ProductsVM();
            productsVM.ProductName = _product.ProductName;
            productsVM.ProductPrice = _product.ProductPrice;
            productsVM.ProductValuePromotion = _product.ProductValuePromotion;
            productsVM.ProductUseful = _product.ProductUseful;
            productsVM.ProductUserManual = _product.ProductUserManual;
            productsVM.ProductIngredient = _product.ProductIngredient;
            productsVM.ProductDescription = _product.ProductDescription;
            productsVM.ProductDetailDescription = _product.ProductDetailDescription;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", productsVM.CategoryId);
            return View(productsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(string id, [FromForm]ProductsVM productsVM)
        {
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if( _product == null) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    _product.ProductName = productsVM.ProductName;
                    _product.ProductPrice = productsVM.ProductPrice;
                    _product.ProductValuePromotion= productsVM.ProductValuePromotion;
                    _product.ProductIngredient= productsVM.ProductIngredient;
                    _product.ProductUseful = productsVM.ProductUseful;
                    _product.ProductUserManual = productsVM.ProductUserManual;
                    _product.ProductDescription = productsVM.ProductDescription;
                    _product.ProductDetailDescription= productsVM.ProductDetailDescription;

                    if (productsVM.IFormFileImage != null && productsVM.IFormFileImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(_product.ProductImage))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(_product.ProductImage));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await productsVM.IFormFileImage.CopyToAsync(stream);
                            }
                            _product.ProductImage = "/images/" + uniqueFileName;

                        }
                        catch
                        {
                            return BadRequest("Lỗi khi lưu trữ hình ảnh: ");
                        }
                    }
                    else
                    {
                        _product.ProductImage = "";
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                   return BadRequest();
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", productsVM.CategoryId);
            return View();
        }

        public async Task<IActionResult> DeleteProduct(string? id)
        {
            var _product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (_product == null)
            {
                return NotFound();
            }
            return View(_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> Delete(string id)
        {
            var _product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (_product == null)
            {
                return NotFound();
            }
            _product.ProductStatus = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
