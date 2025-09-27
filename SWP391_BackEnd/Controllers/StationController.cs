using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    public class StationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
