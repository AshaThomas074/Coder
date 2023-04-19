namespace Coder.Models
{
    public class StudentStatusViewModel
    {
        public string? StudentName { get; set; }
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? Score { get; set; }
        public string? NumberOfTestCasesPassed { get; set; }
        public string? TotalTestCases { get; set; }
        public bool IsStarted { get; set; }
        public int? QuestionSubmittedStatus { get; set; }
        public int? ContestSubmittedStatus { get; set; }        
        public int? ContestId { get; set; }
    }
}
