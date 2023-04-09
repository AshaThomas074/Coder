using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class QuestionContestMap
    {
        [Key]
        public int QuestionContestId { get; set; }        
        public int ContestId { get; set; }
        public Contest? Contest { get; set; }
        public int QuestionId { get; set; }
        public Question? Questions { get; set; }
        public int StartedCount { get; set; }
        //public int ProcessingCount { get; set; }
        public int CompletedCount { get; set; }
        public DateTime CreatedOn { get; set; }= DateTime.Now;
        public DateTime UpdatedOn { get; set; }= DateTime.Now;
        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [NotMapped]
        public string? QuestionHeading { get; set; }
        public ICollection<Submission>? Submissions { get; set; }

    }
}
