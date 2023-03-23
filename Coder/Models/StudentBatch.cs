using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    public class StudentBatch
    {
        [Key]
        public int StudentBatchId { get; set; }
        public string? StudentBatchName { get; set; }
        public int? Status { get; set; }
        [NotMapped]
        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
