using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace Coder.Controllers
{
    public class ContestsController : Controller
    {
        private readonly CoderDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContestsController(CoderDBContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return View(await _context.Contest.Where(x=>x.UserId == userId).OrderByDescending(y=>y.Status).ToListAsync());
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contest == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(HttpContext.User);

            var contest = await _context.Contest.FirstOrDefaultAsync(m => m.ContestId == id);
            if(contest != null)
            { 
                var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "@UserId",
                            SqlDbType =  System.Data.SqlDbType.VarChar,
                            Size = 100,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = userId
                        },
                        new SqlParameter() {
                            ParameterName = "@ContestId",
                            SqlDbType =  System.Data.SqlDbType.Int,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = id
                        }};
                List<Question> questionList = _context.Question.FromSqlRaw("[dbo].[GetQuestionsNotMapped] @UserId, @ContestId", param).ToList();

                contest.questionsDDL = questionList.Select(x => new SelectListItem()
                {
                    Text = x.QuestionHeading,
                    Value = x.QuestionId.ToString()
                });

                contest.questionList = _context.Question.FromSqlRaw("[dbo].[GetQuestionsMapped] @UserId, @ContestId", param).ToList();
            }
            else
            {
                return NotFound();
            }

            return View(contest);
        }

        // GET: Contests/Create
        public IActionResult Create()
        {
            Contest contest = new Contest();
            contest.UserId= _userManager.GetUserId(HttpContext.User);
            return View(contest);
        }

        // POST: Contests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contest contest)
        {
            if (ModelState.IsValid)
            {
                contest.CreatedOn= DateTime.Now;
                contest.UpdatedOn= DateTime.Now;
                contest.Status = 1;
                contest.PublishedStatus = 0;
                _context.Add(contest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contest);
        }

        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contest == null)
            {
                return NotFound();
            }

            var contest = await _context.Contest.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return View(contest);
        }

        // POST: Contests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contest contest)
        {
            if (id != contest.ContestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contest.UpdatedOn = DateTime.Now;
                    _context.Contest.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.ContestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contest);
        }

        //delete contests
        // GET: Contests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contest == null)
            {
                return NotFound();
            }

            var contest = await _context.Contest.FindAsync(id);
            if (contest != null)
            {                
                _context.Contest.Remove(contest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));            
        }

        public async Task<IActionResult> MapQuestionToContest(QuestionContestMap questionContestMap)
        {
            questionContestMap.CreatedOn= DateTime.Now;
            questionContestMap.UpdatedOn= DateTime.Now;
            questionContestMap.UserId= _userManager.GetUserId(HttpContext.User);
            _context.QuestionContestMap.Add(questionContestMap);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details",new { id = questionContestMap.ContestId });

        }


        public async Task<IActionResult> DeleteQuestionContestMaping(int Cid,int Qid)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var questioncontestmap = await _context.QuestionContestMap.
                Where(x => x.ContestId == Cid && x.QuestionId == Qid && x.UserId == userid).
                FirstOrDefaultAsync();
            if (questioncontestmap != null)
            {
                _context.QuestionContestMap.Remove(questioncontestmap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = Cid });
        }

        public async Task<IActionResult> GetMappedStudents(int? id)
        {
            ContestViewModel contestViewModel = new ContestViewModel();
            contestViewModel.ddlStudentBatches = _context.StudentBatch.Select(x => new SelectListItem
            {
                Text = x.StudentBatchName,
                Value = x.StudentBatchId.ToString()
            });

            //Gel all students who are not mapped to contest
            var loggedinId=_userManager.GetUserId(HttpContext.User);
            var allusers = await _userManager.GetUsersInRoleAsync("Student");
            var students = allusers.Where(x => x.CreatedBy == loggedinId).ToList();

            var result = (from a in students
                          where !(from b in _context.StudentContestMap
                                  where b.ContestId == id
                                  select b.UserId)
                                 .Contains(a.Id)
                          select new SelectListItem()
                          {
                              Text = a.UserExternalId + " " + a.FirstName + " " + a.LastName,
                              Value = a.Id
                          });

            contestViewModel.ddlStudents = result;
           
            //Get all mapped students
            contestViewModel.StudentsList = (from a in _context.StudentContestMap
                                             where a.ContestId == id
                                             select new StudentContestMap()
                                             {
                                                 StudentContestId = a.StudentContestId,
                                                 FirstName = a.User != null ? a.User.FirstName : "",
                                                 LastName = a.User != null ? a.User.LastName : "",
                                                 UserExternalId = a.User != null ? a.User.UserExternalId : ""
                                             });

            contestViewModel.StudentContestMap = new StudentContestMap();
            contestViewModel.StudentContestMap.ContestId= id;

            return View("StudentsMap",contestViewModel);
        }
        public async Task<IActionResult> MapStudentToContest(ContestViewModel viewModel)
        {
            int? contestid = null;
            if (viewModel != null && viewModel.StudentContestMap != null)
            {
                contestid = viewModel.StudentContestMap.ContestId;
                if (viewModel.StudentBatchId != null && viewModel.StudentBatchId != 0) 
                {
                    await _context.Database.ExecuteSqlAsync($"InsertStudentContestMapByBranchId @ContestId={contestid},@BatchId={viewModel.StudentBatchId}");
                }
                else if (viewModel.StudentContestMap.UserId != null)
                {
                    
                    viewModel.StudentContestMap.CreatedOn = DateTime.Now;
                    viewModel.StudentContestMap.UpdatedOn = DateTime.Now;

                    _context.StudentContestMap.Add(viewModel.StudentContestMap);
                    await _context.SaveChangesAsync();
                }
            }
            
            return RedirectToAction("GetMappedStudents", new { id = contestid });
        }

        public async Task<IActionResult> DeleteStudentContestMaping(int id, int cid)
        {
            var result=await _context.StudentContestMap.FindAsync(id);
            if(result != null)
            {
                _context.StudentContestMap.Remove(result);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("GetMappedStudents", new { id = cid });
        }

        private bool ContestExists(int id)
        {
          return _context.Contest.Any(e => e.ContestId == id);
        }
    }
}
