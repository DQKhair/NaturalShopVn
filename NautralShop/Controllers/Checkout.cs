using FlowerStore.MyConfiguration;
using Microsoft.AspNetCore.Mvc;
using NaturalShop.Models;
using NaturalShop.Services;
using NautralShop.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Net;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
                            //send mail to customer
                            await _mail.SendEmailAsyncForProduct(dataInfoCus.Email, subject, totalMoneyOrder, dataCart,dataInfoCus);
                            //send mail to employee
                            await _mail.SendEmailAsyncForEmployee(totalMoneyOrder, dataCart, dataInfoCus);
                            //Clear session cart and infoCus
                            HttpContext.Session.Remove("infCheckout");
                            HttpContext.Session.Remove("cart");
                            //transaction.Commit();
                            TempData["SuccessMessage"] = "Đặt hàng thành công";
                            if (paymentMethod == 3)
                            {
                                var url = UrlPayment(0,1, order.OrderId);
                                return Redirect(url);
                            }else
                            {
                                return RedirectToAction(nameof(CheckoutOrderSuccess));
                            }
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
            MyConfiguration myConfiguration = new MyConfiguration();
            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = myConfiguration.Vnp_HashSecret; //Chuoi bi mat
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (var key in vnpayData.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, vnpayData[key]);
                    }
                }

                string orderId = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.Query["vnp_SecureHash"];
                String TerminalID = Request.Query["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.Query["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = _context.Orders.FirstOrDefault(x => x.OrderId == orderId);
                        if (itemOrder != null)
                        {
                            itemOrder.StatusOrderId = 2; // Đã thanh toán
                            _context.Orders.Update(itemOrder);
                            _context.SaveChanges();
                        }
                        TempData["SuccessMessage"] = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    }
                }
            }
            return View();
        }
        public IActionResult CheckoutOrderFail()
        {
            return View();
        }

        public string GetIpAddress(HttpContext context)
        {
            IPAddress? remoteIPAddress = context.Connection.RemoteIpAddress;
            string ipAddress="";
            if(remoteIPAddress != null)
            {
                if(remoteIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIPAddress = Dns.GetHostEntry(remoteIPAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }    
                ipAddress = remoteIPAddress.ToString();
            }

            return ipAddress;
        }



        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, int locale_Vn, string orderId)
        {
            MyConfiguration myConfiguration = new MyConfiguration();
            var urlPayment = "";
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == orderId);
            //Get Config Info
            string vnp_Returnurl = myConfiguration.Vnp_Returnurl; //URL nhan ket qua tra ve 
            string vnp_Url = myConfiguration.Vnp_Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = myConfiguration.Vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = myConfiguration.Vnp_HashSecret; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Convert.ToInt64(order.OrderTotalPrice * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress(HttpContext));
            if (locale_Vn == 1)
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }
            else if (locale_Vn == 2)
            {
                vnpay.AddRequestData("vnp_Locale", "en");
            }
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}
