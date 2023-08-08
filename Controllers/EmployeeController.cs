using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotNetBrushUp.DataModels;
using DotNetBrushUp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;


namespace DotNetBrushUp.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DotNetBrushUpDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeeController(ILogger<EmployeeController> logger, UserManager<ApplicationUser> userManager, 
            DotNetBrushUpDbContext dbContext,IWebHostEnvironment webHostEnvironment) 
        {
            this._logger = logger;
            this._userManager = userManager;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeDetail(EmployeeDetail employeeDetail)
        {
            if (ModelState.IsValid)
            {
                if (employeeDetail.EmployeeProofFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "EmployeeFiles");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeDetail.EmployeeProofFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await employeeDetail.EmployeeProofFile.CopyToAsync(fileStream);
                    }

                    employeeDetail.EmployeeProofFilePath = filePath;
                    employeeDetail.EmployeeProofFileName = uniqueFileName;
                }

                _dbContext.EmployeeDetails.Add(employeeDetail);
                await _dbContext.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Home", new { area = "" });

            }

            //return View(employeeDetail);
            return RedirectToAction("Index", "Home", employeeDetail);
        }

    }
}
