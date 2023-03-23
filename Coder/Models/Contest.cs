﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class Contest
    {
        public int ContestId { get; set; }

        [Display(Name ="Contest Name")]
        [Required]
        public string? ContestName { get; set;}

        [Required]
        [Display(Name ="Final Date")]
        public DateTime FinalDate { get; set; } = DateTime.Now;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? Status { get; set; }   
        public int? PublishedStatus { get; set; }    
        public ICollection<QuestionContestMap>? QuestionContestMaps { get; set; }
        [NotMapped]
       
        public QuestionContestMap? QuestionContestMap { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? questionsDDL { get; set; }
        [NotMapped]
        public IEnumerable<Question>? questionList { get; set; }
        public ICollection<StudentContestMap>? StudentContestMaps { get; set; }

    }
}
