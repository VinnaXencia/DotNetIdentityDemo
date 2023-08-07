using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.Data;
using DotNetBrushUp.DataModels;
using DotNetBrushUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace DotNetBrushUp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DotNetBrushUpDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,UserManager<ApplicationUser> userManager, DotNetBrushUpDbContext dbContext)
        {
            _logger = logger;
            this._userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);
            return View();
        }
        public IActionResult AddEmployeeDetail(EmployeeDetail employeeDetail)
        {
            if (ModelState.IsValid)
            {
                _dbContext.EmployeeDetails.Add(employeeDetail);
                _dbContext.SaveChanges();
                return RedirectToAction("Index"); // Redirect to a success page or list of employees
            }
            return View("Index", employeeDetail);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}