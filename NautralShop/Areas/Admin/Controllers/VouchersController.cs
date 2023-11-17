using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class VouchersController : Controller
    {
        private readonly NaturalShopContext _context;

        public VouchersController(NaturalShopContext context) 
        { 
            this._context = context;
        }

        public async Task <IActionResult> Index()
        {
            var _voucher = await _context.GetListVouchers(); 
            return View(_voucher);
        }

        public async Task<IActionResult> DetailVoucher(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var _voucher = await _context.GetVoucherById(id);
            if(_voucher == null)
            {
                return NotFound();
            }
            return View(_voucher);
        }

        public IActionResult createVoucher()
        {
            return View();
        }

        [HttpPost]
        [ActionName("CreateVoucher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoucher([Bind("VoucherName,VoucherValue,VoucherQuantity,VoucherPoint,DateExpire")]Voucher voucher)
        {
            string EmployeeId = "e754ef0f-5e73-4cc1-b87a-a352bea111c8";
            if (voucher != null)
            {
                await _context.AddVoucher(Guid.NewGuid().ToString(), voucher.VoucherName, voucher.VoucherValue, Convert.ToInt32(voucher.VoucherPoint), voucher.VoucherQuantity, Convert.ToDateTime(voucher.DateExpire), EmployeeId);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("Error", "Lỗi thêm voucher");
            }
            return View();
        }

        public async Task<IActionResult> EditVoucher(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }    
            var _voucher = await _context.GetVoucherById(id);
            if(_voucher == null)
            {
                return NotFound();
            }    
            return View(_voucher);
        }

        [HttpPost]
        [ActionName("EditVoucher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoucher(string id, Voucher voucher)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var _voucher = await _context.GetVoucherById(id);
            if(_voucher == null)
            {
                return NotFound();
            }else
            {
                await _context.EditVoucher(id, voucher.VoucherName, voucher.VoucherValue, Convert.ToInt32(voucher.VoucherPoint), voucher.VoucherQuantity,Convert.ToDateTime(voucher.DateExpire));
                return RedirectToAction(nameof(Index));
            }    
        }

        public async Task<IActionResult> DeleteVoucher(string? id)
        {
            if(id == null)
            {
                return NotFound();
;            }
            var _voucher = await _context.GetVoucherById(id);
            if( _voucher == null)
            {
                return NotFound();
            }
            return View(_voucher);
        }

        [HttpPost]
        [ActionName("DeleteVoucher")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var _voucher = await _context.GetVoucherById(id);
            if( _voucher == null)
            {
                return BadRequest();
            }else
            {
               await _context.DeleteVoucher(id);
                return RedirectToAction(nameof(Index));
            }    
        }
    }
}
