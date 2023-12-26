using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using System.Security.Claims;

namespace NaturalShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Policy = "AdminAndEmployee")]
    public class InfomationsController : Controller
    {
        private readonly NaturalShopContext _context;

        public InfomationsController(NaturalShopContext context)
        {
            this._context = context;
        }

        public IActionResult Index(string employeeId)
        {
            var employee = _context.Employees.Include(ac => ac.AccountType).SingleOrDefault(e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeInfomation(Employee employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }
            var _employee = await _context.Employees.SingleOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
            if (_employee == null)
            {
                return BadRequest();
            }
            _employee.EmployeeName = employee.EmployeeName;
            _employee.EmployeePhone = employee.EmployeePhone;
            _employee.EmployeeAddress = employee.EmployeeAddress;
            _employee.EmployeeEmail = employee.EmployeeEmail;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Lưu thông tin thành công";
            return RedirectToAction(nameof(Index),new {employeeId = _employee.EmployeeId});
        }

        [HttpPost("admin/ChangePassword/employeeId={employeeId}&&oldPass={oldPass}&&newPass={newPass}")]
        public async Task<IActionResult> ChangePassword(string employeeId, string oldPass, string newPass)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            if(BCrypt.Net.BCrypt.Verify(oldPass, employee.EmployeePassword))
            {

                    string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                    employee.EmployeePassword = BCrypt.Net.BCrypt.HashPassword(newPass, salt);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
                    return Json(Ok());                   
            }
            else
            {
                return BadRequest("Mật khẩu cũ không đúng");
            }    
        }
    }
}
