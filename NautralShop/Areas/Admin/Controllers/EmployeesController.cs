using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Areas.Admin.Models;
using NautralShop.Models;
using NuGet.Protocol.Plugins;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class EmployeesController : Controller
    {
        private readonly NaturalShopContext _context;

        public EmployeesController(NaturalShopContext context) 
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var _employees = await _context.GetlistEmployees();
            if(_employees == null)
            {
                return BadRequest();
            }
            return View(_employees);
        }
        public async Task<IActionResult> DetailEmployee(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }    
            var _employee = await _context.GetEmployeesById(id);
            if(_employee == null)
            {
                return NotFound();
            }    
            return View(_employee);
        }

        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ActionName("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(EmployeeVM employeeVM)
        {
            if(employeeVM == null)
            {
                return BadRequest();
            }
            if (employeeVM.EmployeePassword != null && employeeVM.EmployeeUsername != null)
            {
                string lname = employeeVM.EmployeeLName;
                string fname = employeeVM.EmployeeFName;
                string pass = employeeVM.EmployeePassword!;
                string repass = employeeVM.EmployeeRePassword!;
                string username = employeeVM.EmployeeUsername!;
                string email = employeeVM.EmployeeEmail!;

                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                if (pass != repass)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng !");
                    return View();
                }
                else
                {
                    var _employeeCheck = await _context.GetEmployeeByUserName(username);
                    var _employeeCheckEmail = await _context.GetEmployeeByEmail(email);
                    if (_employeeCheck == null && _employeeCheckEmail == null)
                    {
                        await _context.AddEmployee(Guid.NewGuid().ToString(), lname + fname, employeeVM.EmployeeAddress??"", employeeVM.EmployeeEmail?? "", employeeVM.EmployeePhone ?? "", employeeVM.EmployeeUsername!, BCrypt.Net.BCrypt.HashPassword(pass, salt));
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc email đã tồn tại !");
                        return View();
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> EditEmployee(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }    
            var _employee = await _context.GetEmployeesById(id);
            return View(_employee);
        }
        [HttpPost]
        [ActionName("EditEmployee")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            await _context.ResetPassword(id);
            return RedirectToAction(nameof(Index));

        }
        
        
    }
}
