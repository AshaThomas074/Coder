using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coder.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoderDBContext _coderDBContext;
        
        public QuestionController(UserManager<ApplicationUser> userManager,CoderDBContext coderDBContext)
        {
            _userManager = userManager;
            _coderDBContext = coderDBContext;
        }

        [Authorize]
        // GET: QuestionController
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _userManager.FindByIdAsync(userId).Result;

                string temp = HttpContext.Session.GetString("users");
                var users = JsonConvert.DeserializeObject<List<string>>(temp);
                if (users == null)
                {
                    users = new List<string>
                    {
                        userId
                    };
                }
                    
                var questions=await _coderDBContext.Question.Where(x => users.Contains(x.UserId) && x.Status == 1).ToListAsync(); //questions by teacherid for teacher
                return View(questions);
            }
            catch (Exception )
            {
                return View();
            }
        }

        // GET: QuestionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var question = await _coderDBContext.Question.FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // GET: QuestionController/Create
        public ActionResult Create()
        {
            Question question = new()
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                difficulties = _coderDBContext.QuestionDifficulty.Select(x => new SelectListItem
                {
                    Text = x.DifficultyName,
                    Value = x.DifficultyId.ToString()
                }),
                LanguagesList = _coderDBContext.Language.ToList()
            };

            return View(question);
        }

        // POST: QuestionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    question.CreatedOn = DateTime.Now;
                    question.UpdatedOn = DateTime.Now;
                    question.Status = 1;

                    _coderDBContext.Question.Add(question);
                    await _coderDBContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }                
                question.difficulties = _coderDBContext.QuestionDifficulty.Select(x => new SelectListItem
                {
                    Text = x.DifficultyName,
                    Value = x.DifficultyId.ToString()
                });
                return View(question);                
            }
            catch
            {
                question.difficulties = _coderDBContext.QuestionDifficulty.Select(x => new SelectListItem
                {
                    Text = x.DifficultyName,
                    Value = x.DifficultyId.ToString()
                });
                return View(question);
            }
        }

        // GET: QuestionController/Edit/5
        public ActionResult Edit(int id)
        {
            Question? question = _coderDBContext.Question.Where(X => X.QuestionId == id).FirstOrDefault();
            if (question != null)
            {
                question.difficulties = _coderDBContext.QuestionDifficulty.Select(x => new SelectListItem
                {
                    Text = x.DifficultyName,
                    Value = x.DifficultyId.ToString()
                });
            }
            return View(question);
        }

        // POST: QuestionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if(id != question.QuestionId)
            {
                return NotFound();
            }

                if (ModelState.IsValid)
                {
                    try
                    {
                        question.UpdatedOn = DateTime.Now;

                        _coderDBContext.Question.Update(question);
                        await _coderDBContext.SaveChangesAsync();                        
                    }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionId))
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
            return View(question);

        }

        // GET: QuestionController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                Question? question = _coderDBContext.Question.FindAsync(id).Result;
                if(question != null)
                {                    
                    _coderDBContext.Remove(question);
                    await _coderDBContext.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _coderDBContext.Question.Any(e => e.QuestionId == id);
        }
    }
}
