using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Newtonsoft.Json;

namespace Coder.Controllers
{
    [Authorize]
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

            string temp = HttpContext.Session.GetString("users");
            var users = JsonConvert.DeserializeObject<List<string>>(temp);

            if (users == null)
            {
                users = new List<string>
                    {
                        userId
                    };
            }           

            return View(await _context.Contest.Where(x => users.Contains(x.UserId)).OrderByDescending(y => y.Status).ToListAsync());
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contest == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(HttpContext.User);
            string temp = HttpContext.Session.GetString("users");
            var users = JsonConvert.DeserializeObject<List<string>>(temp);
            if (users == null)
            {
                users = new List<string>
                    {
                        userId
                    };
            }            

            var contest = await _context.Contest.FirstOrDefaultAsync(m => m.ContestId == id);
            if(contest != null)
            { 
               /* var param = new SqlParameter[] {
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
                        }};   */            
                //all questions which is not mapped
                contest.questionsDDL = (from a in _context.Question
                                        where users.Contains(a.UserId) &&
                                        !(from b in _context.QuestionContestMap
                                          where users.Contains(b.UserId) && b.ContestId == id
                                          select b.QuestionId).Contains(a.QuestionId)
                                        select new SelectListItem
                                        {
                                            Text = a.QuestionHeading + "(" + (a.QuestionDifficulty != null ? a.QuestionDifficulty.DifficultyName : "") + ")",
                                            Value = a.QuestionId.ToString()
                                        });

                //get all questions which is mapped
                contest.questionList = (from a in _context.Question
                                        where users.Contains(a.UserId) &&
                                        (from b in _context.QuestionContestMap
                                         where users.Contains(b.UserId) && b.ContestId == id
                                         select b.QuestionId).Contains(a.QuestionId)
                                        select a).ToList();
                //contest.questionList = _context.Question.FromSqlRaw("[dbo].[GetQuestionsMapped] @UserId, @ContestId", param).ToList();
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
            if (contest != null && contest.PublishedStatus != 1)
            {
                var studConMap = _context.StudentContestMap.Where(x => x.ContestId == id).ToList();
                if(studConMap != null && studConMap.Count > 0)
                {
                    foreach(var stud in studConMap)
                    {
                        _context.StudentContestMap.Remove(stud);
                    }
                }

                var questMap = _context.QuestionContestMap.Where(x => x.ContestId == id).ToList();
                if (questMap != null && questMap.Count > 0)
                {
                    foreach (var quest in questMap)
                    {
                        _context.QuestionContestMap.Remove(quest);
                    }
                }

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
            string temp = HttpContext.Session.GetString("users");
            var users = JsonConvert.DeserializeObject<List<string>>(temp);
            if (users == null)
            {
                users = new List<string>
                    {
                        userid
                    };
            }
            
            var questioncontestmap = await _context.QuestionContestMap.
                Where(x => x.ContestId == Cid && x.QuestionId == Qid && users.Contains(x.UserId)).
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
            int totalQuestion = 0;
            if (viewModel != null && viewModel.StudentContestMap != null)
            {
                contestid = viewModel.StudentContestMap.ContestId;
                if (viewModel.StudentBatchId != null && viewModel.StudentBatchId != 0) 
                {
                    await _context.Database.ExecuteSqlAsync($"InsertStudentContestMapByBranchId @ContestId={contestid},@BatchId={viewModel.StudentBatchId}");
                }
                else if (viewModel.StudentContestMap.UserId != null)
                {
                    var question = _context.QuestionContestMap.Where(x => x.ContestId == contestid).ToList();
                    if(question != null)
                    {
                        totalQuestion=question.Count;
                    }
                    viewModel.StudentContestMap.CreatedOn = DateTime.Now;
                    viewModel.StudentContestMap.UpdatedOn = DateTime.Now;
                    viewModel.StudentContestMap.TotalQuestions = totalQuestion;
                    viewModel.StudentContestMap.QuestionsAttended = 0;
                    viewModel.StudentContestMap.Status = 0;
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

        [HttpPost]
        public ActionResult PublishContest(int contestId)
        {
            var result = 0;
           
            var totalQuestion = _context.QuestionContestMap.Where(x => x.ContestId == contestId).ToList().Count;
            var contest = _context.Contest.FirstOrDefaultAsync(x => x.ContestId == contestId).Result;
            if (contest != null)
            {
                contest.PublishedStatus = 1;
                contest.UpdatedOn= DateTime.Now;
                _context.Update(contest);
                _context.SaveChanges();
                result = 1;
            }

            var studentContest = _context.StudentContestMap.Where(x => x.ContestId == contestId).ToList();
            foreach(var map in studentContest)
            {
                map.TotalQuestions= totalQuestion;
                map.QuestionsAttended= 0;
                _context.StudentContestMap.Update(map);
                _context.SaveChanges();
            }

            return Json(result);
        }
        
    }
}
