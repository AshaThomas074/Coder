using Microsoft.AspNetCore.Identity;

namespace Coder.Models
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set;}
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public string UserExternalId { get; set; }

    }
}
