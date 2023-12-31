﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;
using System.Security.Claims;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
	[Authorize(Policy = "AdminAndEmployee")]
	public class VouchersController : Controller
    {
        private readonly NaturalShopContext _context;

        public VouchersController(NaturalShopContext context) 
        { 
            this._context = context;
        }

        public async Task <IActionResult> Index(string? search)
        {
           var _voucher = await _context.GetListVouchers(search??"");
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
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var EmployeeId = claimsIdentity!.FindFirst("employeeId")!.Value;
            if (voucher != null)
            {
                await _context.AddVoucher(Guid.NewGuid().ToString(), voucher.VoucherName, voucher.VoucherValue, Convert.ToInt32(voucher.VoucherPoint), voucher.VoucherQuantity, Convert.ToDateTime(voucher.DateExpire), EmployeeId);
                TempData["SuccessMessage"] = "Voucher đã được tạo";
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
                TempData["SuccessMessage"] = "Lưu chỉnh sửa voucher thành công";
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
                TempData["SuccessMessage"] = "Xóa voucher thành công";
                return RedirectToAction(nameof(Index));
            }    
        }
    }
}
