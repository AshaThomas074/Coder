using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public string? SubmissionContent { get; set;}
        public int? NumberOfTestCasesPassed { get; set; }
        public int? TotalTestCases { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public double? Score { get; set; }
        [ForeignKey("QuestionContestId")]
        public int QuestionContestId { get; set; }
        public QuestionContestMap? QuestionContest { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set;}  
        public int? SubmittedStatus { get; set; }
        
    }
}
