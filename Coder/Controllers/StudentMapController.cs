using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    public class StudentMapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
