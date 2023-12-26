using Microsoft.AspNetCore.Mvc;
using NaturalShop.Models;
using NaturalShop.Services;
using NautralShop.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace NaturalShop.Controllers
{
    public class Checkout : Controller
    {
        private readonly NaturalShopContext _context;
        private readonly Mail _mail;

        public Checkout(NaturalShopContext context, Mail mail)
        {
            this._context = context;
            this._mail = mail;
        }
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var customerId = claimsIdentity.FindFirst("CustomerId")?.Value;
                var customer = _context.Customers.SingleOrDefault(c => c.CustomerId == customerId);
                ViewBag.Customer = customer;
            }
            return View();
        }

        public IActionResult CheckoutInFCus([FromForm] IFormCollection form) 
        {


            try
            {
                if(form.Count > 0)
                {
                    var infCheckout = HttpContext.Session.GetString("infCheckout");
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    var customerId = claimsIdentity?.FindFirst("customerId")?.Value??"";
                    if (infCheckout == null)
                    {
                        InfomationCustomerCheckout inf = new InfomationCustomerCheckout();
                        inf.FName = form["fNameCustomer"]!;
                        inf.LName = form["lNameCustomer"]!;
                        inf.Phone = form["phoneCustomer"]!;
                        inf.Email = form["emailCustomer"]!;
                        inf.AddressStreet = form["inputAddressStreet"]!;
                        inf.AddressWard = form["inputAddressWard"]!;
                        inf.AddressDistrict = form["inputAddressDistrict"]!;
                        inf.AddressCity = form["inputAddressCity"]!;
                        inf.PaymentMethod = Convert.ToInt32(form["inputPayMethod"]);
                        inf.CustomerId = customerId;

                        HttpContext.Session.SetString("infCheckout", JsonConvert.SerializeObject(inf));
                        return RedirectToAction(nameof(CheckoutOrder));
                    }
                    else
                    {
                        HttpContext.Session.Remove("infCheckout");

                        InfomationCustomerCheckout inf = new InfomationCustomerCheckout();
                        inf.FName = form["fNameCustomer"]!;
                        inf.LName = form["lNameCustomer"]!;
                        inf.Phone = form["phoneCustomer"]!;
                        inf.Email = form["emailCustomer"]!;
                        inf.AddressStreet = form["inputAddressStreet"]!;
                        inf.AddressWard = form["inputAddressWard"]!;
                        inf.AddressDistrict = form["inputAddressDistrict"]!;
                        inf.AddressCity = form["inputAddressCity"]!;
                        inf.PaymentMethod = Convert.ToInt32(form["inputPayMethod"]);
                        inf.CustomerId = customerId;

                        HttpContext.Session.SetString("infCheckout", JsonConvert.SerializeObject(inf));
                        return RedirectToAction(nameof(CheckoutOrder));
                    }
                }    
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View();
        }

        public IActionResult CheckoutOrder()
        {
            var cart = HttpContext.Session.GetString("cart");
            var inf = HttpContext.Session.GetString("infCheckout");
            if(cart != null && inf !=null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart) ?? new List<Cart>();
                InfomationCustomerCheckout datainf = JsonConvert.DeserializeObject<InfomationCustomerCheckout>(inf)?? new InfomationCustomerCheckout();
                if(datainf != null && dataCart.Count > 0) 
                {
                    ViewData["carts"] = dataCart;
                    ViewData["infs"] = datainf;
                    return View();
                }
            }    
            return View();
        }

        [HttpPost]
        [ActionName(nameof(CheckoutOrder))]
        public async Task<IActionResult> ConfirmCheckout()
        {
            var infoCus = HttpContext.Session.GetString("infCheckout");
            var cart = HttpContext.Session.GetString("cart");
            if(infoCus != null && cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart) ?? new List<Cart>();
                InfomationCustomerCheckout dataInfoCus = JsonConvert.DeserializeObject<InfomationCustomerCheckout>(infoCus) ?? new InfomationCustomerCheckout();
                string nameCusOrder = dataInfoCus.LName + " " + dataInfoCus.FName;
                string addressCusOrder = $"{dataInfoCus.AddressStreet}, {dataInfoCus.AddressWard}, {dataInfoCus.AddressDistrict}, {dataInfoCus.AddressCity}";
                string phoneCusOrder = dataInfoCus.Phone;
                string customerId = dataInfoCus.CustomerId??"";
                int paymentMethod = dataInfoCus.PaymentMethod;
                var coutCart = 0;
                int totalMoneyProduct = 0;
                string orderIdCus = Guid.NewGuid().ToString();
                //cart
                foreach (var item in dataCart)
                {
                    coutCart += item.Quantity;
                    totalMoneyProduct += (Convert.ToInt32(item.Quantity) * Convert.ToInt32(item.Product!.ProductPrice - item.Product.ProductValuePromotion));
                }
                int totalMoneyOrder = totalMoneyProduct + 32000;

                if (dataCart.Count > 0 && dataInfoCus != null)
                {
                    //using (var transaction = _context.Database.BeginTransaction())
                    //{
                        try
                        {
                            Order order = new Order();
                        //add vao order
                            order.OrderId = orderIdCus;
                            order.OrderDate = DateTime.Now;
                            order.OrderTotalQuantity = coutCart;
                            order.OrderTotalPrice = totalMoneyOrder;
                            order.OrderCustomerName = nameCusOrder;
                            order.OrderCustomerPhone = phoneCusOrder;
                            order.OrderCustomerAddress = addressCusOrder;
                            if(customerId != "")
                            {
                             order.CustomerId = customerId;
                            }else
                            {
                            order.CustomerId = null;
                            }    
                            order.EmployeeId = null;
                            order.PaymentMethodId = paymentMethod;
                            order.StatusOrderId = 1;
                            //_context.Orders.add(order);
                            _context.Orders.Add(order);
                            foreach (var item in dataCart)
                            {
                                OrderDetail orderDetail = new OrderDetail();
                                //add detail 
                                orderDetail.OrderDetailId = Guid.NewGuid().ToString();
                                orderDetail.OrderDetailQuantity = item.Quantity;
                                orderDetail.OrderDetailPrice = Convert.ToDouble(item.Product!.ProductPrice * item.Quantity);
                                orderDetail.OrderId = orderIdCus;
                                orderDetail.ProductId = item.Product.ProductId;
                                //_context.DetailOrder.add(DetailOrder);
                                _context.OrderDetails.Add(orderDetail);

                            //handle minus quantity product
                                var product = _context.Products.Where(p => p.ProductId == item.Product.ProductId).SingleOrDefault();
                                if(product != null)
                                {
                                    product.ProductQuantity -= item.Quantity;

                                }
                            }
                            //_context.SaveChange();
                            await _context.SaveChangesAsync();
                            string subject = "Đơn hàng đã đặt";
                            await _mail.SendEmailAsyncForProduct(dataInfoCus.Email, subject, totalMoneyOrder, dataCart,dataInfoCus);
                            //Clear session cart and infoCus
                            HttpContext.Session.Remove("infCheckout");
                            HttpContext.Session.Remove("cart");
                            //transaction.Commit();
                            TempData["SuccessMessage"] = "Đặt hàng thành công";
                        return RedirectToAction(nameof(CheckoutOrderSuccess));
                        }
                        catch
                        {
                            //transaction.Rollback();
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    //}    
                }else
                {
                    return RedirectToAction(nameof(CheckoutOrderFail));
                }

            }else
            {
                return RedirectToAction(nameof(CheckoutOrderFail));
            }
        }

        public IActionResult CheckoutOrderSuccess()
        {
            return View();
        }
        public IActionResult CheckoutOrderFail()
        {
            return View();
        }
    }
}
