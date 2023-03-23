using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class StudentContestMap
    {
        [Key]
        public int StudentContestId { get; set; }
        public int? ContestId { get; set; }
        public Contest? Contest { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? TotalQuestions { get; set; }
        public int? QuestionsAttended { get; set; }
        public double? TotalEarnedScore { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get;set; }
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? UserExternalId { get; set; }
        
        

    }
}
