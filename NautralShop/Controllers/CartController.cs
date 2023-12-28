using Microsoft.AspNetCore.Mvc;
using NaturalShop.Models;
using NautralShop.Models;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace NaturalShop.Controllers
{
    public class CartController : Controller
    {
        private readonly NaturalShopContext _context;


        public CartController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart)?? new List<Cart>();
                if(dataCart.Count > 0)
                {
                    ViewData["carts"] = dataCart;
                    return View();
                }
            }
            return View();
        }

        [HttpPost("/AddToCart/productId={productId}&&quantity={quantity}")]
        public async Task<IActionResult> AddToCart(string productId, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");   
            if(cart == null)
            {
                var _product = await _context.GetProductById(productId);
                List<Cart> listCart = new List<Cart>()
                {
                    new Cart
                    {
                        Product = _product,
                        Quantity = quantity
                    }
                };              
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
            }
            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart)?? new List<Cart>();
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product?.ProductId == productId)
                    {
                        dataCart[i].Quantity += quantity;
                        check = false;
                    }
                }
                if (check)
                {
                    var _product = await _context.GetProductById(productId);
                    dataCart.Add(new Cart
                    {

                        Product = _product,
                        Quantity = quantity
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng";
            return Json(Ok("Thêm vào giỏ thành công"));
        }

        [HttpPost("/updateCart/productId={productId}&&quantity={quantity}")]
        public IActionResult UpdateCart(string productId, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart)?? new List<Cart>();
                if (quantity > 0)
                {
                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        if (dataCart[i].Product!.ProductId == productId)
                        {
                            dataCart[i].Quantity = quantity;
                        }
                    }


                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                }
                var cart2 = HttpContext.Session.GetString("cart");
                TempData["SuccessMessage"] = "Số lượng đã được cập nhật";
                return Ok(quantity);
            }
            return BadRequest();

        }

        public IActionResult DeleteCart(string productId)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart)?? new List<Cart>();

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product!.ProductId == productId)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
