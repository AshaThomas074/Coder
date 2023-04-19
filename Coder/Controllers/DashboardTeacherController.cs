using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    public class DashboardTeacherController : Controller
    {
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public DashboardTeacherController(CoderDBContext coderDBContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _coderDBContext = coderDBContext;
            _userManager = userManager;
            _configuration = configuration;
        }
        public IActionResult Index(int? id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid);

            IEnumerable<Contest> contest = (from a in _coderDBContext.Contest
                                            where a.PublishedStatus == 1 &&
                                            a.UserId == userid
                                            select new Contest
                                            {
                                                ContestId = a.ContestId,
                                                ContestName = a.ContestName,
                                                FinalDate = a.FinalDate,
                                                PublishedStatus = a.PublishedStatus,
                                                Status = a.Status,
                                                DivOpenStatus = a.ContestId == id ? true : false,
                                                QuestionContestList = (from b in _coderDBContext.QuestionContestMap
                                                                       where b.ContestId == a.ContestId &&
                                                                        (from c in _coderDBContext.Question
                                                                         select c.QuestionId).Contains(b.QuestionId)
                                                                       select new QuestionContestMap
                                                                       {
                                                                           QuestionHeading = b.Questions != null ? b.Questions.QuestionHeading : "",
                                                                           StartedCount = b.StartedCount,
                                                                           CompletedCount = b.CompletedCount,
                                                                           QuestionId = b.QuestionId,
                                                                           QuestionContestId= b.QuestionContestId
                                                                       }).ToList()
                                            });

            return View(contest);            
        }

        public IActionResult GetLeaderBoard(int id)
        {
            var result = (from a in _coderDBContext.StudentContestMap
                          where a.ContestId == id && a.Status == 1
                          select new LeaderBoardModel
                          {
                              UserId = a.UserId,
                              StudentName = a.User != null ? a.User.FirstName + " " + a.User.LastName : "",
                              ExternalId = a.User != null ? a.User.UserExternalId : "",
                              Score = a.TotalEarnedScore,
                              TimeTaken = (a.CompletedOn - a.StartedOn),
                              Percent = a.QuestionsAttended != null ? (double)a.QuestionsAttended / a.TotalQuestions * 100 : 0,
                          }).ToList().OrderByDescending(b => b.Score).OrderBy(c => c.TimeTaken);
            return View("LeaderBoard", result);
        }

        public IActionResult ViewStudentStatus(int id)
        {
            int contestId = 0;
            var contest = _coderDBContext.QuestionContestMap.Where(x => x.QuestionContestId == id).FirstOrDefault();
            if(contest != null)
            {
                contestId = contest.ContestId;
            }
            var result1 = (from a in _coderDBContext.Submission
                          where a.QuestionContestId == id
                          select new StudentStatusViewModel
                          {
                              StudentName = a.User != null ? a.User.FirstName + " " + a.User.LastName : "",
                              UserId = a.UserId,
                              ContestId = contestId,
                              ExternalId = a.User != null ? a.User.UserExternalId : "",
                              Score = a.Score == null ? "N/A" : a.Score.ToString(),
                              NumberOfTestCasesPassed = a.NumberOfTestCasesPassed == null ? "N/A" : a.NumberOfTestCasesPassed.ToString(),
                              TotalTestCases = a.TotalTestCases == null ? "N/A" : a.TotalTestCases.ToString(),
                              IsStarted = a.SubmissionId != 0 ? true : false,
                              QuestionSubmittedStatus = a.SubmittedStatus,
                              ContestSubmittedStatus = _coderDBContext.StudentContestMap.Where(x => x.UserId == a.UserId && x.ContestId == contestId).FirstOrDefault().Status 
                          }).ToList();

            var result2 = (from a in _coderDBContext.StudentContestMap
                           where a.ContestId == contestId &&
                           !(from b in _coderDBContext.Submission
                             where b.QuestionContestId == id
                             select b.UserId).Contains(a.UserId)
                           select new StudentStatusViewModel
                           {
                               StudentName = a.User != null ? a.User.FirstName + " " + a.User.LastName : "",
                               UserId = a.UserId,
                               ContestId = contestId,
                               ExternalId = a.User != null ? a.User.UserExternalId : "",
                               Score = "N/A",
                               NumberOfTestCasesPassed = "N/A",
                               TotalTestCases = "N/A",
                               IsStarted = false,
                               QuestionSubmittedStatus = 0,
                               ContestSubmittedStatus = 0
                           }).ToList();

            var result = result1.Union(result2).OrderBy(x=>x.StudentName);
            return View("StudentQuestionStatus",result);
        }
    }
}
