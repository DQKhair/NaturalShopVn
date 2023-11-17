using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace NautralShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly NaturalShopContext _context;

        public LoginController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            if(claimsUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                //handle login
                var _customer = await _context.GetCustomerByUsername(username);
                if (_customer != null && BCrypt.Net.BCrypt.Verify(password, _customer!.CustomerPassword))
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role,"Customer"),
                        new Claim(ClaimTypes.Name,_customer.CustomerName),
                        new Claim("CustomerId",_customer.CustomerId)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = false,
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Index", "Home");
                }
                var _employee = await _context.GetEmployeeByUserName(username);
                if (_employee != null && BCrypt.Net.BCrypt.Verify(password, _employee!.EmployeePassword))
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role,"Employee"),
                        new Claim(ClaimTypes.Name,_employee.EmployeeName),
                        new Claim("employeeId",_employee.EmployeeId)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = false,
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Index", "PageAdmin",new {area = "admin"});
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            //handle logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng !");
                    return View("SignUp");
                }
                else
                {
                    var _customerCheck = await _context.GetCustomerByUsername(username);
					var _customerCheckEmail = await _context.GetEmployeeByEmail(email);
					if (_customerCheck == null && _customerCheckEmail == null)
                    {
                        await _context.SignupCustomer(Guid.NewGuid().ToString(), lname +" "+ fname, customerVM.CustomerAddress??"", customerVM.CustomerEmail??"", customerVM.CustomerPhone??"", customerVM.CustomerUsername!, BCrypt.Net.BCrypt.HashPassword(pass, salt));
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

	}
}
