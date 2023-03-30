using Coder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coder.Data
{
    public class CoderDBContext : IdentityDbContext<ApplicationUser>
    {
        public CoderDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> users { get; set; }
        public DbSet<QuestionDifficulty> QuestionDifficulty { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Contest> Contest { get; set; }
        public DbSet<QuestionContestMap> QuestionContestMap { get; set; }
        public DbSet<StudentBatch> StudentBatch { get; set; }
        public DbSet<StudentContestMap> StudentContestMap { get; set;}
        public DbSet<Language> Language { get; set; }
        public DbSet<Submission> Submission { get; set; }
    }
}
