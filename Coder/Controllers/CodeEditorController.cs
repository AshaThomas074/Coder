using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    public class CodeEditorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
