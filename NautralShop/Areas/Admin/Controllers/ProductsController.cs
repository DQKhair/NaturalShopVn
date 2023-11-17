using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
using NautralShop.Models;
using System.Net.WebSockets;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
			ViewData["ProductList"] = await _context.GetListProducts();
			return View();
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
                    string ProductImage = "/images/" + uniqueFileName;
                     await _context.AddProduct(Guid.NewGuid().ToString(), productsVM.ProductName, (double)productsVM.ProductPrice!, ProductImage,true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0),productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);

                }catch
                {
                    return BadRequest("Lỗi khi lưu trữ hình ảnh: ");
                }
            }else
            {
                string emtyProductImage = "";
                await _context.AddProduct(Guid.NewGuid().ToString(), productsVM.ProductName, (double)productsVM.ProductPrice!, emtyProductImage, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", productsVM.CategoryId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditProduct(string? id)
        {
            if(String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var _product = await _context.GetProductById(id);
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
            var _product = await _context.GetProductById(id);
            if( _product == null) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
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
                                string ProductImage = "/images/" + uniqueFileName;
                                await _context.EditProduct(id, productsVM.ProductName, (double)productsVM.ProductPrice!, ProductImage, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
                                return RedirectToAction(nameof(Index));
                        }
                        catch
                        {
                            return BadRequest("Lỗi khi lưu trữ hình ảnh: ");
                        }
                    }
                    else
                    {
                            string EmptyProductImage = "";
                            await _context.EditProduct(id, productsVM.ProductName, (double)productsVM.ProductPrice!, EmptyProductImage, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                   return BadRequest();
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatgoryName", productsVM.CategoryId);
            return View();
        }

        public async Task<IActionResult> DeleteProduct(string? id)
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
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> Delete(string id)
        {
            var _product = await _context.GetProductById(id);
            if (_product == null)
            {
                return NotFound();
            }else
            {
               await _context.DeleteProduct(id);
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
