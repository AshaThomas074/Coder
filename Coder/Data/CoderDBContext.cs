using Coder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coder.Data
{
    public class CoderDBContext : IdentityDbContext
    {
        public CoderDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
    }
}
