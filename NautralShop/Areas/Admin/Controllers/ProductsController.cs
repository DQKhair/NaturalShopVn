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
            var _products = await _context.Products.Include(c=>c.Category).ToListAsync();
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
            return View();
        }
    }
}
