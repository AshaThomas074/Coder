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
        public DateTime FinalDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set;}
        public string? TeacherId { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public ICollection<Question>? QuestionsList { get; set; }
    }
}
