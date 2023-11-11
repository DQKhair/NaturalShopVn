using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NautralShop.Models;

namespace NautralShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PageAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
