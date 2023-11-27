using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NautralShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NaturalShopContext _context;

        public HomeController(ILogger<HomeController> logger, NaturalShopContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                var userName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

                ViewBag.UserName = userName;
                ViewBag.Role = role;
            }
            var _productCate1 = await _context.GetListProductWithCateId(1);
            ViewData["productCate1"] = _productCate1;
            var _productCate2 = await _context.GetListProductWithCateId(2);
            ViewData["productCate2"] = _productCate2;
            var _productCate3 = await _context.GetListProductWithCateId(3);
            ViewData["productCate3"] = _productCate3;
            var _productCate4 = await _context.GetListProductWithCateId(4);
            ViewData["productCate4"] = _productCate4;
            return View();
        }
        public async Task<IActionResult> DetailProduct(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }   
            var _productDetail = await _context.GetProductById(id);
            if(_productDetail == null)
            {
                return BadRequest();
            }
            return View(_productDetail);
        }

        public  IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}