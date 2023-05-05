using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Coder.Controllers
{
    public class ViewStudentsController : Controller
    {
        private readonly CoderDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ViewStudentsController(CoderDBContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            
                ContestViewModel contestViewModel = new ContestViewModel();
                contestViewModel.ddlStudentBatches = _context.StudentBatch.Select(x => new SelectListItem
                {
                    Text = x.StudentBatchName,
                    Value = x.StudentBatchId.ToString()
                });

           
                return View(contestViewModel);
            
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContestViewModel? model = null)
        {

            ContestViewModel contestViewModel = new ContestViewModel();
            contestViewModel.ddlStudentBatches = _context.StudentBatch.Select(x => new SelectListItem
            {
                Text = x.StudentBatchName,
                Value = x.StudentBatchId.ToString()
            });

            if (model != null)
            {
                contestViewModel.StudentBatchId = model.StudentBatchId;
            }   

            //Gel all students who are not mapped to contest
            var loggedinId = _userManager.GetUserId(HttpContext.User);
            string temp = HttpContext.Session.GetString("users");
            var users = JsonConvert.DeserializeObject<List<string>>(temp);

            if (users == null)
            {
                users = new List<string>
                    {
                        loggedinId
                    };
            }

            var allusers = await _userManager.GetUsersInRoleAsync("Student");
            var students = allusers.Where(x => users.Contains(x.CreatedBy)).ToList();

            var result = (from a in students
                          where a.StudentBatchId == contestViewModel.StudentBatchId
                          select new StudentContestMap()
                          {
                              FirstName = a.FirstName,
                              LastName = a.LastName,
                              UserExternalId = a.UserExternalId
                          }).ToList();

            contestViewModel.StudentsList = result;
            return View(contestViewModel);

        }
    }
}
