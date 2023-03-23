using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coder.Models
{
    [NotMapped]
    public class ContestViewModel
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        public string? UserExternalId { get; set; }
        public int? StudentBatchId { get; set; }
        public StudentContestMap? StudentContestMap { get; set; }
        public IEnumerable<SelectListItem>? ddlStudents { get; set; }
        public IEnumerable<SelectListItem>? ddlStudentBatches { get; set; }
        public IEnumerable<StudentContestMap>? StudentsList { get; set; }
    }
}
