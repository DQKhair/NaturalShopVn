using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class VouchersController : Controller
    {
        private readonly NaturalShopContext _context;

        public VouchersController(NaturalShopContext context) 
        { 
            this._context = context;
        }

        public async Task <IActionResult> Index()
        {
            var _voucher = await _context.Vouchers.ToListAsync(); 
            return View(_voucher);
        }

        public IActionResult createVoucher()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoucher(Voucher voucher)
        {
            if(ModelState.IsValid)
            {
                var _voucher = new Voucher();
                _voucher.VoucherId = Guid.NewGuid().ToString();
                _voucher.VoucherName = voucher.VoucherName;
                _voucher.VoucherValue = voucher.VoucherValue;
                _voucher.VoucherPoint = voucher.VoucherPoint;
                _voucher.VoucherQuantity = voucher.VoucherQuantity;
                _voucher.DateExpire = DateTime.Now;
                _voucher.EmployeeId = null;
                _context.Vouchers.Add(_voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }    
            return View();
        }
    }
}
