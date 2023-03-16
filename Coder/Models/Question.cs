using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        [Required]
        [Display(Name="Question Heading")]
        public string? QuestionHeading { get; set; }

        [Required]
        [Display(Name ="Enter your question")]
        public string? QuestionText { get; set; }

        [ValidateNever]
        [Display(Name = "Input 1")]
        public string TestCaseInput1 { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Output 1")]
        public string? TestCaseOutput1 { get; set; }

        [ValidateNever]
        [Display(Name ="Input 2")]
        public string? TestCaseInput2 { get; set;}

        [Display(Name = "Output 2")]
        public string? TestCaseOutput2 { get; set; }

        [Display(Name = "Input 3")]
        public string? TestCaseInput3 { get; set; }
        [Display(Name = "Output 3")]
        public string? TestCaseOutput3 { get; set; }

        [Required]
        [Display(Name ="Initial Template")]
        public string? Answer { get; set; }

        [Required]
        [Display(Name = "Final Date")]
        public DateTime? FinalDate { get; set; }
        [Required]
        public float? Score { get; set; }

        [Required]
        [Display(Name = "Difficulty")]

        [ForeignKey(nameof(QuestionDifficulty))]
        public int Difficulty { get; set; }
        public QuestionDifficulty? QuestionDifficulty { get; set; }

        [Display(Name = "Started Count")]
        public int StartedCount { get; set; }
        [Display(Name = "Processing Count")]
        public int ProcessedCount { get; set; }
        [Display(Name = "Completed Count")]
        public int CompletedCount { get; set; }

        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public int Status { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? difficulties { get; set; }
        
        public ICollection<QuestionContestMap>? QuestionContestMaps { get; set; }
    }
}
