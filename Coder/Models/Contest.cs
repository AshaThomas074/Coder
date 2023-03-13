using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class Contest
    {
        public int ContestId { get; set; }
        [Display(Name ="Contest Name")]
        public string? ContestName { get; set;}
        [Display(Name ="Final Date")]
        public DateTime FinalDate { get; set; } = DateTime.Now;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Status { get; set; }
        public ICollection<QuestionContestMap> QuestionContestMaps { get; set; }

    }
}
