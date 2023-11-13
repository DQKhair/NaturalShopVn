using Microsoft.AspNetCore.Mvc;
using NautralShop.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                //handle login
                var _user = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerUsername == username);
                if (username != null && BCrypt.Net.BCrypt.Verify(password, _user!.CustomerPassword))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            //handle logout
			return RedirectToAction("Login", "Login");
		}

        public IActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> SignUp(CustomerVM customerVM)
		{
            if (ModelState.IsValid)
            {
                string lname = customerVM.CustomerLastName;
                string fname = customerVM.CustomerFirstName;
                string pass = customerVM.CustomerPassword!;
                string repass = customerVM.CustomerRePassword!;

                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                if (pass != repass)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng !");
                    return View("SignUp");
                }
                else
                {
                    var _customer = new Customer();
                    _customer.CustomerId = Guid.NewGuid().ToString();
                    _customer.CustomerName = lname + fname;
                    _customer.CustomerPhone = customerVM.CustomerPhone;
                    _customer.CustomerAddress = customerVM.CustomerAddress;
                    _customer.CustomerEmail = customerVM.CustomerEmail;
                    _customer.CustomerUsername = customerVM.CustomerUsername;
                    _customer.CustomerPassword = BCrypt.Net.BCrypt.HashPassword(pass, salt);
                    _customer.CustomerPoint = 0;
                    _customer.CustomerStatus = true;
                    _customer.AccountTypeId = 3;

                    _context.Customers.Add(_customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
                }
            }
            return View();
		}

	}
}
