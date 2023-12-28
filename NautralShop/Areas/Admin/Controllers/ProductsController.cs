using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
using NautralShop.Models;
using System.Net.WebSockets;
using X.PagedList;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class ProductsController : Controller
    {
        private readonly NaturalShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(NaturalShopContext context, IWebHostEnvironment _webHostEnvironment) 
        {
            this._context = context;
            this._webHostEnvironment = _webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? page,string? search)
        {
			var product = await _context.Products.Where(p => p.ProductStatus == true && p.ProductName.Contains(search??"")).Include(c => c.Category).ToListAsync();
            var pageSize = 8;
            var pageNumber = page ?? 1;
            var pageList = product.ToPagedList(pageNumber, pageSize);
            ViewData["ProductList"] = pageList;
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
            if(productsVM.IFormFileImage != null && productsVM.IFormFileImage.Length > 0 && productsVM.IFormFileImage2 != null && productsVM.IFormFileImage2.Length > 0 && productsVM.IFormFileImage3 != null && productsVM.IFormFileImage3.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage.FileName;
                var uniqueFileName2 = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage2.FileName;
                var uniqueFileName3 = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage3.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                var filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);
                var filePath3 = Path.Combine(uploadsFolder, uniqueFileName3);
                try
                {
                    using( var stream = new FileStream(filePath,FileMode.Create))
                    {
                        await productsVM.IFormFileImage.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePath2, FileMode.Create))
                    {
                        await productsVM.IFormFileImage2.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePath3, FileMode.Create))
                    {
                        await productsVM.IFormFileImage3.CopyToAsync(stream);
                    }
                    string ProductImage = "/images/" + uniqueFileName;
                    string ProductImage2 = "/images/" + uniqueFileName2;
                    string ProductImage3 = "/images/" + uniqueFileName3;
                    await _context.AddProduct(Guid.NewGuid().ToString(), productsVM.ProductName, (double)productsVM.ProductPrice!,productsVM.ProductQuantity, ProductImage, ProductImage2, ProductImage3, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0),productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
                    TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công";

                }
                catch
                {
                    return BadRequest("Lỗi khi lưu trữ hình ảnh ");
                }
            }else
            {
                string emtyProductImage = "";
                await _context.AddProduct(Guid.NewGuid().ToString(), productsVM.ProductName, (double)productsVM.ProductPrice!,productsVM.ProductQuantity, emtyProductImage, emtyProductImage, emtyProductImage, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
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
            ViewBag.ProductVMId = id;
            ProductsVM productsVM = new ProductsVM();
            productsVM.ProductName = _product.ProductName;
            productsVM.ProductPrice = _product.ProductPrice;
            productsVM.ProductQuantity = _product.ProductQuantity;
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
                    if (productsVM.IFormFileImage != null && productsVM.IFormFileImage.Length > 0 && productsVM.IFormFileImage2 != null && productsVM.IFormFileImage2.Length > 0 && productsVM.IFormFileImage3 != null && productsVM.IFormFileImage3.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(_product.ProductImage) && !string.IsNullOrEmpty(_product.ProductImage2) && !string.IsNullOrEmpty(_product.ProductImage3))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(_product.ProductImage));
                            var oldImagePath2 = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(_product.ProductImage2));
                            var oldImagePath3 = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(_product.ProductImage3));
                            if (System.IO.File.Exists(oldImagePath) && System.IO.File.Exists(oldImagePath2) && System.IO.File.Exists(oldImagePath3))
                            {
                                System.IO.File.Delete(oldImagePath);
                                System.IO.File.Delete(oldImagePath2);
                                System.IO.File.Delete(oldImagePath3);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage.FileName;
                        var uniqueFileName2 = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage2.FileName;
                        var uniqueFileName3 = Guid.NewGuid().ToString() + "_" + productsVM.IFormFileImage3.FileName;

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        var filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);
                        var filePath3 = Path.Combine(uploadsFolder, uniqueFileName3);
                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await productsVM.IFormFileImage.CopyToAsync(stream);
                            }
                            using (var stream = new FileStream(filePath2, FileMode.Create))
                            {
                                await productsVM.IFormFileImage2.CopyToAsync(stream);
                            }
                            using (var stream = new FileStream(filePath3, FileMode.Create))
                            {
                                await productsVM.IFormFileImage3.CopyToAsync(stream);
                            }

                            string ProductImage = "/images/" + uniqueFileName;
                            string ProductImage2 = "/images/" + uniqueFileName2;
                            string ProductImage3 = "/images/" + uniqueFileName3;

                            await _context.EditProduct(id, productsVM.ProductName, (double)productsVM.ProductPrice!,productsVM.ProductQuantity, ProductImage, ProductImage2, ProductImage3, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
                            TempData["SuccessMessage"] = "Thông tin sản phẩm đã lưu thành công";
                            return RedirectToAction(nameof(Index));
                        }
                        catch
                        {
                            return BadRequest("Lỗi khi lưu trữ hình ảnh");
                        }
                    }
                    else
                    {
                            string EmptyProductImage = "";
                            await _context.EditProduct(id, productsVM.ProductName, (double)productsVM.ProductPrice!, productsVM.ProductQuantity, EmptyProductImage, EmptyProductImage, EmptyProductImage, true, Convert.ToDouble(productsVM.ProductValuePromotion ?? 0), productsVM.ProductIngredient ?? "", productsVM.ProductUseful ?? "", productsVM.ProductUserManual ?? "", productsVM.ProductDescription ?? "", productsVM.ProductDetailDescription ?? "", (int)productsVM.CategoryId!);
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
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> EditTextProduct(string? id)
        {
            if (String.IsNullOrEmpty(id))
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
            productsVM.ProductQuantity = _product.ProductQuantity;
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
        public async Task<IActionResult> EditTextProduct(string? id, [FromForm] ProductsVM productsVM)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var _product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if (_product == null)
            {
                return NotFound();
            }
            _product.ProductName = productsVM.ProductName;
            _product.ProductPrice = productsVM.ProductPrice;
            _product.ProductValuePromotion = productsVM.ProductValuePromotion;
            _product.ProductQuantity = productsVM.ProductQuantity;
            _product.CategoryId = productsVM.CategoryId;
            _product.ProductIngredient = productsVM.ProductIngredient;
            _product.ProductUseful = productsVM.ProductUseful;
            _product.ProductUserManual = productsVM.ProductUserManual;
            _product.ProductDescription = productsVM.ProductDescription;
            _product.ProductDetailDescription = productsVM.ProductDetailDescription;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thông tin sản phẩm đã được lưu";
            return RedirectToAction(nameof(Index));
        }

    }
}
