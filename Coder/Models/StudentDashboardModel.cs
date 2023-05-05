namespace Coder.Models
{
    public class StudentDashboardModel
    {
        public IEnumerable<Contest>? Contests { get; set; }
        public ResponseModel? Response { get; set; }
    }
}
