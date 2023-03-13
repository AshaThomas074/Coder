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

            var contest = await _context.Contest.FirstOrDefaultAsync(m => m.ContestId == id);
            if (contest == null)
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

        private bool ContestExists(int id)
        {
          return _context.Contest.Any(e => e.ContestId == id);
        }
    }
}
