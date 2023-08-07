using Microsoft.AspNetCore.Mvc;

namespace DotNetBrushUp.Controllers
{
    public class EmployeeController : Controller
    {
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
