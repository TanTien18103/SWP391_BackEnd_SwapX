using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    public class BatteryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
