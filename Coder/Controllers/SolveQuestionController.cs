using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    public class SolveQuestionController : Controller
    {
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolveQuestionController(CoderDBContext coderDBContext,UserManager<ApplicationUser> userManager)
        {
            _coderDBContext = coderDBContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user=_userManager.FindByIdAsync(userid);
            IEnumerable<Contest> contest = (from a in _coderDBContext.Contest                                            
                          where a.PublishedStatus==1 && 
                          (from b in _coderDBContext.StudentContestMap
                                 where b.UserId == userid
                                 select b.ContestId
                                 ).Contains(a.ContestId)                                 
                          select a).ToList();
            return View(contest);
        }

        public IActionResult ViewQuestions(int id)
        {
            IList<QuestionViewModel> question = (from a in _coderDBContext.Question
                                                 where (from b in _coderDBContext.QuestionContestMap
                                                        where b.ContestId == id
                                                        select b.QuestionId).Contains(a.QuestionId)
                                                 select new QuestionViewModel()
                                                 {
                                                     QuestionId = a.QuestionId,
                                                     QuestionHeading = a.QuestionHeading != null ? a.QuestionHeading : "",
                                                     Difficulty = a.Difficulty != 0 && a.QuestionDifficulty != null ? a.QuestionDifficulty.DifficultyName : "",
                                                     Score = a.Score
                                                 }).ToList();
                
            return View(question);
        }

        public async Task<IActionResult> LoadSolveChallengePage(int id)
        {
            var question=await _coderDBContext.Question.FindAsync(id);
            return View("SolveQuestion",question);
        }
    }
}
