using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public string? SubmissionContent { get; set;}
        public int? NumberOfTestCasesPassed { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? applicationUser;
        public double? Score { get; set; }
        public int? QuestionContestId { get; set; }
        public QuestionContestMap? QuestionContestMap { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set;}        
        
    }
}
