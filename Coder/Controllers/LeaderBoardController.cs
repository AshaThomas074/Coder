using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    public class LeaderBoardController : Controller
    {
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public LeaderBoardController(CoderDBContext coderDBContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _coderDBContext = coderDBContext;
            _userManager = userManager;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid);

            IEnumerable<Contest> contest = (from a in _coderDBContext.Contest
                                            where a.PublishedStatus == 1 &&
                                            (from b in _coderDBContext.StudentContestMap
                                             where b.UserId == userid
                                             select b.ContestId
                                                   ).Contains(a.ContestId)
                                            select new Contest
                                            {
                                                ContestId = a.ContestId,
                                                ContestName = a.ContestName,
                                                FinalDate = a.FinalDate,
                                                PublishedStatus = a.PublishedStatus,
                                                Status = a.Status                                                
                                            });
            return View(contest);
        }

        public IActionResult GetLeaderBoard(int id)
        {
            var result = (from a in _coderDBContext.StudentContestMap
                          where a.ContestId == id && a.Status == 1
                          select new LeaderBoardModel
                          {
                              UserId=a.UserId,
                              StudentName=a.User != null ? a.User.FirstName+" "+a.User.LastName : "",
                              ExternalId= a.User != null ? a.User.UserExternalId : "",
                              Score=a.TotalEarnedScore,
                              TimeTaken=(a.CompletedOn-a.StartedOn),
                              Percent = a.QuestionsAttended != null ? (double)a.QuestionsAttended / a.TotalQuestions * 100 : 0,
                          }).ToList().OrderByDescending(b=>b.Score).OrderBy(c=>c.TimeTaken);
            return View("LeaderBoard",result);
        }
    }
}
