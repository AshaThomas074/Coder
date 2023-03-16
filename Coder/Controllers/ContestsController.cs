using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ContestsController(CoderDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return View(await _context.Contest.Where(x=>x.UserId == userId).ToListAsync());
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
                //contest.QuestionContestMaps = (ICollection<QuestionContestMap>)x.ToList();
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

        public async Task<IActionResult> DeleteMaping(int Cid,int Qid)
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

        private bool ContestExists(int id)
        {
          return _context.Contest.Any(e => e.ContestId == id);
        }
    }
}
