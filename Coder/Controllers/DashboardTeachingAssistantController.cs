using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coder.Controllers
{
    [Authorize]
    public class DashboardTeachingAssistantController : Controller
    {
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardTeachingAssistantController(CoderDBContext coderDBContext, UserManager<ApplicationUser> userManager)
        {
            _coderDBContext = coderDBContext;
            _userManager = userManager;
        }
        public IActionResult Index(int? id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid);
            var teacherid = user.Result.CreatedBy;

            IEnumerable<Contest> contest = (from a in _coderDBContext.Contest
                                            where a.PublishedStatus == 1 &&
                                            (a.UserId == userid)
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
                                                                           QuestionContestId = b.QuestionContestId
                                                                       }).ToList()
                                            });

            return View(contest);
        }

    }
}
