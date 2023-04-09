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
                                                     Score = a.Score,
                                                     SubmittedStatus= (from c in _coderDBContext.Submission
                                                                       where (from d in _coderDBContext.QuestionContestMap
                                                                              where d.ContestId == id && d.QuestionId == a.QuestionId
                                                                              select d.QuestionContestId).Contains(c.QuestionContestId)
                                                                              select c.SubmittedStatus).FirstOrDefault()
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

            viewModel.LanguagesList = GetLanguages();
            viewModel.ContestId = cid;

            if (TempData["Error"] != null)
                viewModel.Error = (string)TempData["Error"];
            if (TempData["Success"] != null)
                viewModel.Success = (int)TempData["Success"];
            
            var questionContestId =await _coderDBContext.QuestionContestMap.Where(x => x.ContestId == cid && x.QuestionId == qid).Select(y => y.QuestionContestId).FirstOrDefaultAsync();
            var userId= _userManager.GetUserId(HttpContext.User);

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
                viewModel.Submission = new Submission();
                viewModel.Submission.UserId = userId;
                viewModel.Submission.QuestionContestId = questionContestId;
            }

            //add contest attended time - start
            var studentContest = _coderDBContext.StudentContestMap.FirstOrDefault(x => x.UserId == userId && x.ContestId == cid);
            if(studentContest != null)
            {
                if(studentContest.AttendedOn == null)
                {
                    studentContest.AttendedOn = DateTime.Now;
                    studentContest.UpdatedOn = DateTime.Now;
                    _coderDBContext.Update(studentContest);
                    await _coderDBContext.SaveChangesAsync();
                }
            }
            //add contest attended time - end

            TempData["Error"] = null;
            TempData["Success"] = null;
            TempData["NumberOfTestCasesPassed"] = null;
            TempData["TotalTestCases"] = null;
            return View("SolveQuestion", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompileCode(SolveQuestionViewModel viewModel)
        {
            JDoodleModel jConObj = new JDoodleModel();
            jConObj.clientId = _configuration.GetValue<string>("clientId");
            jConObj.clientSecret = _configuration.GetValue<string>("clientSecret");
            var jdoodleAPI = _configuration.GetValue<string>("jdoodleAPI");

            if(viewModel.Submission!= null)
            {
                var submission = viewModel.Submission;
                if(submission.SubmissionId != 0) //update submissions
                {
                    submission.UpdatedOn = DateTime.Now;
                    _coderDBContext.Update(submission);
                    await _coderDBContext.SaveChangesAsync();
                }
                else
                {
                    submission.CreatedOn=DateTime.Now;
                    submission.UpdatedOn=DateTime.Now;
                    submission.SubmittedStatus = 0;
                    _coderDBContext.Add(submission);
                    await _coderDBContext.SaveChangesAsync();
                }

                if(viewModel.Question != null)
                {
                    var questions = await _coderDBContext.Question.FirstOrDefaultAsync(x => x.QuestionId == viewModel.Question.QuestionId);
                    jConObj.script = submission.SubmissionContent;
                    jConObj.language = viewModel.JDoodleLanguageCode;
                    jConObj.versionIndex = viewModel.VersionIndex;

                    using var client=new HttpClient();

                    if (questions != null)
                    {
                        if (!string.IsNullOrEmpty(questions.SampleInput))
                        {
                            jConObj.stdin = questions.SampleInput;
                        }

                        string json = JsonConvert.SerializeObject(jConObj);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(jdoodleAPI, data);
                        var resp = await response.Content.ReadAsStringAsync();
                        dynamic result = JObject.Parse(resp);
                        string res = result.output != null ? result.output.ToString() : "";
                        string sampleoutput = !string.IsNullOrEmpty(questions.SampleOutput) ? questions.SampleOutput.Replace("\r\n", "\n") : "";
                        string returnoutput = result.output;
                        
                       
                        if (!string.IsNullOrEmpty(res) && res.Contains("error"))
                        {
                            viewModel.Error = result.output;
                            jConObj.ErrorMessage = viewModel.Error;
                            TempData["Error"] = result.output;
                        }
                        else if ((!string.IsNullOrEmpty(sampleoutput) && !string.IsNullOrEmpty(returnoutput)) && returnoutput.TrimEnd('\r','\n',' ') == sampleoutput.TrimEnd('\r','\n',' '))
                        {
                            jConObj.Success = 1;
                            TempData["Success"] = 1;
                        }

                        //Update challenge started count- start
                        var questionContestModel = _coderDBContext.QuestionContestMap.FirstOrDefault(x => x.QuestionContestId == submission.QuestionContestId);
                        if (questionContestModel != null)
                        {
                            questionContestModel.StartedCount= _coderDBContext.Submission.Where(x=> x.SubmittedStatus != 1).ToList().Count();
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
            JDoodleModel jConObj = new JDoodleModel();
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
                            jConObj.TestOutput= questions.TestCaseOutput2;
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
                            submission.TotalTestCases= noOfTestCases;
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
                        if(questionContestModel != null)
                        {
                            questionContestModel.CompletedCount++;
                            questionContestModel.UpdatedOn = DateTime.Now;
                            _coderDBContext.Update(questionContestModel);
                            await _coderDBContext.SaveChangesAsync();
                        }

                        //return RedirectToAction("LoadSolveChallengePage", new { qid = viewModel.Question.QuestionId, cid = viewModel.ContestId, success = viewModel.Success });
                        return RedirectToAction("LoadSolveChallengePage", new { qid = viewModel.Question.QuestionId, cid = viewModel.ContestId });
                    }
                }
            }

            return RedirectToAction("Index");
        }

        private async Task<SolveQuestionViewModel> JDoodleAPICall(JDoodleModel jConObj)
        {
            SolveQuestionViewModel viewModel = new SolveQuestionViewModel();
            jConObj.clientId = _configuration.GetValue<string>("clientId");
            jConObj.clientSecret = _configuration.GetValue<string>("clientSecret");
            var jdoodleAPI = _configuration.GetValue<string>("jdoodleAPI");
            
            using var client = new HttpClient();
            string json = JsonConvert.SerializeObject(jConObj);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(jdoodleAPI, data);
            var resp = await response.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(resp);
            string res = result.output != null ? result.output.ToString() : "";
            string sampleoutput = !string.IsNullOrEmpty(jConObj.TestOutput) ? jConObj.TestOutput.Replace("\r\n", "\n") : "";
            string returnoutput = result.output;
            if (!string.IsNullOrEmpty(res) && res.Contains("error"))
            {
                viewModel.Error = result.output;
            }
            else if (!string.IsNullOrEmpty(sampleoutput) && returnoutput.TrimEnd('\r', '\n', ' ') == sampleoutput.TrimEnd('\r', '\n', ' '))
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
    }
}
