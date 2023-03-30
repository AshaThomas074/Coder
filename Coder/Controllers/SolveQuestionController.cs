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
        private readonly IConfiguration _configuration;

        public SolveQuestionController(CoderDBContext coderDBContext,UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _coderDBContext = coderDBContext;
            _userManager = userManager;
            _configuration = configuration;
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
            SolveQuestionViewModel viewModel = new SolveQuestionViewModel();
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

            viewModel.QuestionViewModel = question;
            viewModel.ContestId= id;
            return View(viewModel);
        }

        public async Task<IActionResult> LoadSolveChallengePage(int qid, int cid)
        {
            SolveQuestionViewModel viewModel = new SolveQuestionViewModel();
            var question = await _coderDBContext.Question.FindAsync(qid);
            if (question != null)
                viewModel.Question = question;
            viewModel.LanguagesList = (from a in _coderDBContext.Language
                                      select new Language()
                                      {
                                          LanguageId= a.LanguageId,
                                          LanguageName= a.LanguageName,
                                          JDoodleLanguageCode= a.JDoodleLanguageCode,
                                          AceLanguageCode= a.AceLanguageCode,
                                          InitialCode= a.InitialCode
                                      }).ToList();

            viewModel.Submission = new Submission();
            viewModel.Submission.QuestionContestId = _coderDBContext.QuestionContestMap.Where(x => x.ContestId == cid && x.QuestionId == qid).Select(y => y.QuestionContestId).FirstOrDefault();
            viewModel.Submission.UserId= _userManager.GetUserId(HttpContext.User);
            return View("SolveQuestion", viewModel);
        }

        public IActionResult CompileCode()
        {
            var clientid = _configuration.GetValue<string>("clientId");
            var clientSecret = _configuration.GetValue<string>("clientSecret");
            var jdoodleAPI = _configuration.GetValue<string>("jdoodleAPI");


            return View();
        }

        [HttpGet]
        public JsonResult GetLanguageList()
        {
           var lang= (from a in _coderDBContext.Language
             select new Language()
             {
                 LanguageId = a.LanguageId,
                 LanguageName = a.LanguageName,
                 JDoodleLanguageCode = a.JDoodleLanguageCode,
                 AceLanguageCode = a.AceLanguageCode,
                 InitialCode = a.InitialCode
             }).ToList();
            return Json(lang);
        }
    }
}
