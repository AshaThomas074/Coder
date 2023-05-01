using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Coder.Extensions;
using SessionExtensions = Microsoft.AspNetCore.Http.SessionExtensions;
using Newtonsoft.Json;

namespace Coder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, CoderDBContext coderDBContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _coderDBContext = coderDBContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin"))
                {
                }
                else if (User.IsInRole("Teacher") || User.IsInRole("Teaching Assistant"))
                {
                    GetAccountStatus();
                    return RedirectToAction("Index", "DashboardTeacher");
                }
                else if (User.IsInRole("Student"))
                {
                    return RedirectToAction("Index", "DashboardStudent");
                }
            }
            
            return View();

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

        private void GetAccountStatus()
        {
            List<string> users = new();
            var userId = _userManager.GetUserId(HttpContext.User);

            if (User.IsInRole("Teacher"))
            {
                users = (from a in _coderDBContext.UserRoles
                         where (from c in _coderDBContext.Roles
                                where c.Name == "Teaching Assistant"
                                select c.Id).Contains(a.RoleId)
                         && (from b in _coderDBContext.Users
                             where b.CreatedBy == userId
                             select b.Id).Contains(a.UserId)
                         select a.UserId).ToList();
                users.Add(userId);
            }
            else if (User.IsInRole("Teaching Assistant"))
            {
                var user = _userManager.FindByIdAsync(userId);
                users = (from a in _coderDBContext.UserRoles
                         where (from c in _coderDBContext.Roles
                                where c.Name == "Teaching Assistant"
                                select c.Id).Contains(a.RoleId)
                         && (from b in _coderDBContext.Users
                             where b.CreatedBy == user.Result.CreatedBy
                             select b.Id).Contains(a.UserId)
                         select a.UserId).ToList();
                users.Add(user.Result.CreatedBy);
            }

            string temp = JsonConvert.SerializeObject(users);
            HttpContext.Session.SetString("users", temp);            
        }
    }
}