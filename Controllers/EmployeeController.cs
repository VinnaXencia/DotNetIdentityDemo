using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotNetBrushUp.DataModels;
using DotNetBrushUp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace DotNetBrushUp.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DotNetBrushUpDbContext _dbContext;
        public EmployeeController(ILogger<EmployeeController> logger, UserManager<ApplicationUser> userManager, DotNetBrushUpDbContext dbContext) 
        {
            this._logger = logger;
            this._userManager = userManager;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AddEmployeeDetail()
        {
            return View();
        }
    }
}
