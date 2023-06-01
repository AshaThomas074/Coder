using Coder.Data;
using Coder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coder.Services
{
    public class ContestService
    {
        private readonly CoderDBContext _context;

        public ContestService(CoderDBContext coderDBContext)
        {
            _context = coderDBContext;

        }

        public List<Contest> GetContests(List<string> users)
        {

            return _context.Contest.Where(x => users.Contains(x.UserId)).OrderByDescending(y => y.Status).ToList();
        }

        public int CreateContest(Contest contest)
        {
            contest.CreatedOn = DateTime.Now;
            contest.UpdatedOn = DateTime.Now;
            contest.Status = 1;
            contest.PublishedStatus = 0;
            contest.FinalDate = contest.FinalDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            _context.Add(contest);
            _context.SaveChanges();
            return contest.ContestId;
        }

        public void DeleteContest(int? id)
        {
            var contest = _context.Contest.Find(id);
            if (contest != null && (contest.PublishedStatus != 1 || contest.Status == 0))
            {
                var studConMap = _context.StudentContestMap.Where(x => x.ContestId == id).ToList();
                if (studConMap != null && studConMap.Count > 0)
                {
                    foreach (var stud in studConMap)
                    {
                        _context.StudentContestMap.Remove(stud);
                    }
                }

                var questMap = _context.QuestionContestMap.Where(x => x.ContestId == id).ToList();
                if (questMap != null && questMap.Count > 0)
                {
                    foreach (var quest in questMap)
                    {
                        _context.QuestionContestMap.Remove(quest);
                    }
                }

                _context.Contest.Remove(contest);
            }

            _context.SaveChanges();
        }

    }
}
