using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Text;

namespace Coder.Controllers
{
    public class DashboardStudentController : Controller
    {
        private readonly CoderDBContext _coderDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public DashboardStudentController(CoderDBContext coderDBContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
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
                                                 Status= a.Status,
                                                 AttendedOn = (from c in _coderDBContext.StudentContestMap
                                                               where c.UserId == userid && c.ContestId == a.ContestId
                                                               select c.StartedOn
                                                       ).FirstOrDefault(),
                                                 CompletedOn = (from c in _coderDBContext.StudentContestMap
                                                                where c.UserId == userid && c.ContestId == a.ContestId
                                                                select c.CompletedOn
                                                       ).FirstOrDefault(),
                                                 StudentContestStatus= (from c in _coderDBContext.StudentContestMap
                                                                        where c.UserId == userid && c.ContestId == a.ContestId 
                                                                        select c.Status
                                                       ).FirstOrDefault(),
                                                 Percent= (from c in _coderDBContext.StudentContestMap
                                                           where c.UserId == userid && c.ContestId == a.ContestId
                                                           select ((double)c.QuestionsAttended / c.TotalQuestions)*100 
                                                       ).FirstOrDefault()
                                             });

            return View(contest);
        }

        public IActionResult ViewQuestions(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            SolveQuestionViewModel viewModel = new();
            IList<QuestionViewModel> question = (from a in _coderDBContext.Question
                                                 where (from b in _coderDBContext.QuestionContestMap
                                                        where b.ContestId == id
                                                        select b.QuestionId).Contains(a.QuestionId)
                                                 select new QuestionViewModel()
                                                 {
                                                     QuestionId = a.QuestionId,
                                                     QuestionHeading = a.QuestionHeading ?? "",
                                                     Difficulty = a.Difficulty != 0 && a.QuestionDifficulty != null ? a.QuestionDifficulty.DifficultyName : "",
                                                     Score = a.Score,
                                                     SubmittedStatus = (from c in _coderDBContext.Submission
                                                                        where c.UserId == userId
                                                                        where (from d in _coderDBContext.QuestionContestMap
                                                                               where d.ContestId == id && d.QuestionId == a.QuestionId
                                                                               select d.QuestionContestId).Contains(c.QuestionContestId)
                                                                        select c.SubmittedStatus).FirstOrDefault()
                                                 }).ToList();

            viewModel.QuestionViewModel = question;
            viewModel.ContestId = id;
            var stuConMap = _coderDBContext.StudentContestMap.FirstOrDefault(x => x.ContestId == id && x.UserId == userId);
            if (stuConMap != null)
                viewModel.StudentContestFinishStatus = stuConMap.Status;
            return View(viewModel);
        }

        public async Task<IActionResult> LoadSolveChallengePage(int qid, int cid)
        {
            SolveQuestionViewModel viewModel = new();
            var question = await _coderDBContext.Question.FindAsync(qid);
            if (question != null)
                viewModel.Question = question;

            viewModel.LanguagesList = GetLanguages();
            viewModel.ContestId = cid;

            if (TempData["Error"] != null)
                viewModel.Error = (string)TempData["Error"];
            if (TempData["Success"] != null)
                viewModel.Success = (int)TempData["Success"];

            var questionContestId = await _coderDBContext.QuestionContestMap.Where(x => x.ContestId == cid && x.QuestionId == qid).Select(y => y.QuestionContestId).FirstOrDefaultAsync();
            var userId = _userManager.GetUserId(HttpContext.User);

            var subResult = await _coderDBContext.Submission.Where(x => x.QuestionContestId == questionContestId && x.UserId == userId).FirstOrDefaultAsync();
            if (subResult != null)
            {
                viewModel.Submission = subResult;

                if (TempData["NumberOfTestCasesPassed"] != null)
                    viewModel.Submission.NumberOfTestCasesPassed = (int)TempData["NumberOfTestCasesPassed"];
                if (TempData["TotalTestCases"] != null)
                    viewModel.Submission.TotalTestCases = (int)TempData["TotalTestCases"];
            }
            else
            {
                viewModel.Submission = new Submission
                {
                    UserId = userId,
                    QuestionContestId = questionContestId
                };
            }

            //add contest attended time - start
            var studentContest = _coderDBContext.StudentContestMap.FirstOrDefault(x => x.UserId == userId && x.ContestId == cid);
            if (studentContest != null)
            {
                if (studentContest.StartedOn == null)
                {
                    studentContest.StartedOn = DateTime.Now;
                    studentContest.UpdatedOn = DateTime.Now;
                    _coderDBContext.Update(studentContest);
                    await _coderDBContext.SaveChangesAsync();
                }
            }
            //add contest attended time - end

            var stuConMap = _coderDBContext.StudentContestMap.FirstOrDefault(x => x.ContestId == cid && x.UserId == userId);
            if (stuConMap != null)
                viewModel.StudentContestFinishStatus = stuConMap.Status;

            TempData["Error"] = null;
            TempData["Success"] = null;
            TempData["NumberOfTestCasesPassed"] = null;
            TempData["TotalTestCases"] = null;
            return View("SolveQuestion", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompileCode(SolveQuestionViewModel viewModel)
        {
            JDoodleModel jConObj = new();

            if (viewModel.Submission != null)
            {
                var submission = viewModel.Submission;
                if (submission.SubmissionId != 0) //update submissions
                {
                    submission.UpdatedOn = DateTime.Now;
                    _coderDBContext.Update(submission);
                    await _coderDBContext.SaveChangesAsync();
                }
                else
                {
                    submission.CreatedOn = DateTime.Now;
                    submission.UpdatedOn = DateTime.Now;
                    submission.SubmittedStatus = 0;
                    _coderDBContext.Add(submission);
                    await _coderDBContext.SaveChangesAsync();
                }

                if (viewModel.Question != null)
                {
                    var questions = await _coderDBContext.Question.FirstOrDefaultAsync(x => x.QuestionId == viewModel.Question.QuestionId);
                    jConObj.script = submission.SubmissionContent;
                    jConObj.language = viewModel.JDoodleLanguageCode;
                    jConObj.versionIndex = viewModel.VersionIndex;                   

                    if (questions != null)
                    {
                        if (!string.IsNullOrEmpty(questions.SampleInput))
                        {
                            jConObj.stdin = questions.SampleInput;
                            jConObj.TestOutput = questions.SampleOutput;
                        }

                        SolveQuestionViewModel result = await JDoodleAPICall(jConObj);            
                        if (result != null)
                        {
                            if (result.Success == 1)
                            {
                                TempData["Success"] = 1;
                            }
                            if (result.Error != null)
                            {
                                TempData["Error"] = result.Error;
                            }
                        }
                        //Update challenge started count- start
                        var questionContestModel = _coderDBContext.QuestionContestMap.FirstOrDefault(x => x.QuestionContestId == submission.QuestionContestId);
                        if (questionContestModel != null)
                        {
                            questionContestModel.StartedCount = _coderDBContext.Submission.Where(x => x.SubmittedStatus != 1 && x.QuestionContestId == questionContestModel.QuestionContestId).ToList().Count;
                            questionContestModel.UpdatedOn = DateTime.Now;
                            _coderDBContext.Update(questionContestModel);
                            await _coderDBContext.SaveChangesAsync();
                        }
                        //Update challenge started count- end

                        return RedirectToAction("LoadSolveChallengePage", new { qid = viewModel.Question.QuestionId, cid = viewModel.ContestId });
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitChallenge(SolveQuestionViewModel viewModel)
        {
            JDoodleModel jConObj = new();
            if (viewModel.Submission != null)
            {
                var submission = viewModel.Submission;

                if (viewModel.Question != null)
                {
                    var questions = await _coderDBContext.Question.FirstOrDefaultAsync(x => x.QuestionId == viewModel.Question.QuestionId);
                    jConObj.script = submission.SubmissionContent;
                    jConObj.language = viewModel.JDoodleLanguageCode;
                    jConObj.versionIndex = viewModel.VersionIndex;

                    int noOfTestCases = 0;
                    int testCasesPassed = 0;
                    if (questions != null)
                    {
                        if (!string.IsNullOrEmpty(questions.TestCaseInput1))
                        {
                            jConObj.stdin = questions.TestCaseInput1;
                            jConObj.TestOutput = questions.TestCaseOutput1;
                            noOfTestCases++;

                            SolveQuestionViewModel result = await JDoodleAPICall(jConObj);
                            if (result != null)
                            {
                                if (result.Success == 1)
                                {
                                    testCasesPassed++;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(questions.TestCaseInput2))
                        {
                            jConObj.stdin = questions.TestCaseInput2;
                            jConObj.TestOutput = questions.TestCaseOutput2;
                            noOfTestCases++;

                            SolveQuestionViewModel result = await JDoodleAPICall(jConObj);
                            if (result != null)
                            {
                                if (result.Success == 1)
                                {
                                    testCasesPassed++;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(questions.TestCaseInput3))
                        {
                            jConObj.stdin = questions.TestCaseInput3;
                            jConObj.TestOutput = questions.TestCaseOutput3;
                            noOfTestCases++;

                            SolveQuestionViewModel result = await JDoodleAPICall(jConObj);
                            if (result != null)
                            {
                                if (result.Success == 1)
                                {
                                    testCasesPassed++;
                                }
                            }
                        }

                        TempData["NumberOfTestCasesPassed"] = testCasesPassed;
                        TempData["TotalTestCases"] = noOfTestCases;

                        var score = (testCasesPassed * questions.Score) / noOfTestCases;

                        if (submission.SubmissionId != 0) //update submissions
                        {
                            submission.UpdatedOn = DateTime.Now;
                            submission.SubmittedStatus = 1;
                            submission.NumberOfTestCasesPassed = testCasesPassed;
                            submission.TotalTestCases = noOfTestCases;
                            submission.Score = score;
                            _coderDBContext.Update(submission);
                            await _coderDBContext.SaveChangesAsync();
                        }
                        else
                        {
                            submission.CreatedOn = DateTime.Now;
                            submission.UpdatedOn = DateTime.Now;
                            submission.SubmittedStatus = 0;
                            submission.Score = score;
                            _coderDBContext.Add(submission);
                            await _coderDBContext.SaveChangesAsync();
                        }

                        var questionContestModel = _coderDBContext.QuestionContestMap.FirstOrDefault(x => x.QuestionContestId == submission.QuestionContestId);
                        if (questionContestModel != null)
                        {
                            questionContestModel.CompletedCount++;
                            questionContestModel.StartedCount--;
                            questionContestModel.UpdatedOn = DateTime.Now;
                            _coderDBContext.Update(questionContestModel);
                            await _coderDBContext.SaveChangesAsync();
                        }

                        var map = _coderDBContext.StudentContestMap.Where(x => x.ContestId == viewModel.ContestId && x.UserId == submission.UserId).FirstOrDefault();
                        if (map != null)
                        {
                            var subResult = (from a in _coderDBContext.Submission
                                             where a.UserId == submission.UserId && a.SubmittedStatus == 1 &&
                                             (from b in _coderDBContext.QuestionContestMap
                                              where b.ContestId == viewModel.ContestId
                                              select b.QuestionContestId).Contains(a.QuestionContestId)
                                             select a).ToList();

                            map.QuestionsAttended = subResult.Count;
                            map.TotalEarnedScore = subResult.Sum(x => x.Score);
                            _coderDBContext.Update(map);
                            _coderDBContext.SaveChanges();

                        }
                        return RedirectToAction("LoadSolveChallengePage", new { qid = viewModel.Question.QuestionId, cid = viewModel.ContestId });
                    }
                }
            }

            return RedirectToAction("Index");
        }

        private async Task<SolveQuestionViewModel> JDoodleAPICall(JDoodleModel jConObj)
        {
            SolveQuestionViewModel viewModel = new();
            jConObj.clientId = _configuration.GetValue<string>("clientId");
            jConObj.clientSecret = _configuration.GetValue<string>("clientSecret");
            var jdoodleAPI = _configuration.GetValue<string>("jdoodleAPI");

            using var client = new HttpClient();
            string json = JsonConvert.SerializeObject(jConObj);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(jdoodleAPI, data);
            var resp = await response.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(resp);
            string res = (result != null && result.output != null) ? result.output.ToString() : "";
            string sampleoutput = !string.IsNullOrEmpty(jConObj.TestOutput) ? jConObj.TestOutput.Replace("\r\n", "\n") : "";
            string returnoutput = "";
            if (result != null)
            {
                returnoutput = result.output;
            }
             
            if (!string.IsNullOrEmpty(res) && res.Contains("error"))
            {
                viewModel.Error = result.output;
            }
            else if (!string.IsNullOrEmpty(sampleoutput) && !string.IsNullOrEmpty(returnoutput) && returnoutput.TrimEnd('\r', '\n', ' ') == sampleoutput.TrimEnd('\r', '\n', ' '))
            {
                viewModel.Success = 1;
            }
            return viewModel;
        }

        [HttpGet]
        public JsonResult GetLanguageList()
        {
            return Json(GetLanguages());
        }

        public IList<Language> GetLanguages()
        {
            return (from a in _coderDBContext.Language
                    select new Language()
                    {
                        LanguageId = a.LanguageId,
                        LanguageName = a.LanguageName,
                        JDoodleLanguageCode = a.JDoodleLanguageCode,
                        AceLanguageCode = a.AceLanguageCode,
                        InitialCode = a.InitialCode,
                        VersionIndex = a.VersionIndex
                    }).ToList();

        }

        [HttpPost]
        public JsonResult UpdateStudentContestFinishStatus(int contestId)
        {
            int result = 0;
            var userId = _userManager.GetUserId(HttpContext.User);
            var map=_coderDBContext.StudentContestMap.Where(x=>x.ContestId== contestId && x.UserId == userId).FirstOrDefault();
            if (map != null)
            {                 
                map.CompletedOn=DateTime.Now;
                map.Status = 1;
                _coderDBContext.Update(map);
                result= _coderDBContext.SaveChanges();
            }
            if(result != 0)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
            
        }
    }
}
