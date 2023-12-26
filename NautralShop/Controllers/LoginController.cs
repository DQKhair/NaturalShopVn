using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;
using NaturalShop.Services;

namespace NautralShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly NaturalShopContext _context;
		private readonly Mail _mail;

		public LoginController(NaturalShopContext context,Mail mail) 
        {
            this._context = context;
            this._mail = mail;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
			if (claimsUser != null && claimsUser.Identity != null && claimsUser.Identity.IsAuthenticated)
            {
			    var claimsIdentity = User.Identity as ClaimsIdentity;
			    var role = claimsIdentity!.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "Employee")
                {
                    return RedirectToAction("index", "pageadmin",new {area = "admin"});
                }else if(role == "Customer"){
                    return RedirectToAction("Index", "Home");
                }else
                {
                    return View();
                }    
            }    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                if(userLogin == null || userLogin.Password == null || userLogin.Username == null)
                {
                    return BadRequest();
                }    
                //handle login
                var _customer = await _context.GetCustomerByUsername(userLogin.Username);
                if (_customer != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, _customer!.CustomerPassword))
                {
                    if (_customer.CustomerStatus == true)
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Role,"Customer"),
                        new Claim(ClaimTypes.Name,_customer.CustomerName),
                        new Claim("CustomerId",_customer.CustomerId)
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = userLogin.RememberMe,
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        TempData["SuccessMessage"] = "Đăng nhập thành công";
                        return RedirectToAction("Index", "Home");
                    }else
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa !");
                    }    
                    
                }
                var _employee = await _context.GetEmployeeByUserName(userLogin.Username);
                if (_employee != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, _employee!.EmployeePassword))
                {
                    if (_employee.EmployeeStatus == true)
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Role,"Employee"),
                            new Claim(ClaimTypes.Name,_employee.EmployeeName),
                            new Claim("EmployeeId",_employee.EmployeeId),
                            new Claim("AccountTypeId",_employee.AccountTypeId.ToString()!)
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = userLogin.RememberMe,
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        TempData["SuccessMessage"] = "Đăng nhập thành công";
                        return RedirectToAction("Index", "PageAdmin",new {area = "admin"});
                    }else
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa !");
                    }   
                }
				ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng !");
			}
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            //handle logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			TempData["SuccessMessage"] = "Đăng xuất thành công";
			return RedirectToAction(nameof(Login));
		}

        public IActionResult SignUp()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> SignUp(CustomerVM customerVM)
		{
            if(customerVM == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                string lname = customerVM.CustomerLastName;
                string fname = customerVM.CustomerFirstName;
                string pass = customerVM.CustomerPassword!;
                string repass = customerVM.CustomerRePassword!;
                string username = customerVM.CustomerUsername!;
                string email = customerVM.CustomerEmail!;

                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                if (pass != repass)
                {
                    ModelState.AddModelError("", "Mật khẩu không trùng khớp !");
                    return View("SignUp");
                }
                else
                {
                    var _customerCheck = await _context.GetCustomerByUsername(username);
					var _customerCheckEmail = await _context.GetEmployeeByEmail(email);
					if (_customerCheck == null && _customerCheckEmail == null)
                    {
                        await _context.SignupCustomer(Guid.NewGuid().ToString(), lname +" "+ fname, customerVM.CustomerAddress??"", customerVM.CustomerEmail??"", customerVM.CustomerPhone??"", customerVM.CustomerUsername!, BCrypt.Net.BCrypt.HashPassword(pass, salt));
						TempData["SuccessMessage"] = "Đăng ký thành công";
						return RedirectToAction(nameof(Login));
                    }else
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc email đã tồn tại !");
                        return View("SignUp");
                    }    
                }
            }
            return View();
		}

        //forgot passwork
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string emailForgot)
        {
          try
            {

				if (emailForgot == null)
				{
					return NotFound("Lỗi không tìm thấy");
				}
				var customer = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerEmail!.Equals(emailForgot));
				if (customer == null)
				{
					ModelState.AddModelError("", "Email không tồn tại !");
				}
				else
				{
					string subject = "Reset Mật khẩu";
					string message = "Mật khẩu là Naturalshop123 ";

					if (customer.CustomerEmail != null)
					{
						string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
						await _mail.SendEmailAsync(customer.CustomerEmail, subject, message);

                        customer.CustomerPassword = BCrypt.Net.BCrypt.HashPassword("Naturalshop123",salt);
                        _context.SaveChanges();
						ModelState.AddModelError("", "Mật khẩu đã được gửi vào mail");

						return View();
					}

				}
            return View();

			}catch(Exception ex)
            {
                return View("Error forgot password " + ex);
            }


		}
	}
}
