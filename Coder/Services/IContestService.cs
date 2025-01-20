using Coder.Models;

namespace Coder.Services
{
    public interface IContestService
    {
        List<Contest> GetContests(List<string> users);
        int CreateContest(Contest contest);
        void DeleteContest(int? id);
    }
}
