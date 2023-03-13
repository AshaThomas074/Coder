﻿using Microsoft.AspNetCore.Identity;

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
        public ICollection<QuestionContestMap> ContestMaps { get; set; }
        public ICollection<Contest> Contests { get; set; }
        public ICollection<Question> Questions { get; set; }

    }
}