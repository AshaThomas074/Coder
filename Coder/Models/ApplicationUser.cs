using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public string? UserExternalId { get; set; }
        public string? CreatedBy { get; set; }
        public int? StudentBatchId { get; set; }
        public StudentBatch? StudentBatch { get; set; }
        public ICollection<QuestionContestMap>? ContestMaps { get; set; }
        public ICollection<Contest>? Contests { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<StudentContestMap>? StudentContestMaps { get; set; }
        public ICollection<Submission>? Submissions { get; set;}
        [NotMapped]
        public string? StudentBatchName { get; set; }
        
    }
}
