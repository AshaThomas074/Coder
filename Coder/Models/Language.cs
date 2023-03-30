using System.ComponentModel.DataAnnotations;

namespace Coder.Models
{
    public class Language
    {
        [Key]
        public int LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public string? JDoodleLanguageCode { get; set; }
        public string? AceLanguageCode { get; set; }
        public int? VersionIndex { get; set; }
        public string? InitialCode { get; set; }
        public ICollection<Submission>? Submissions { get; set; }
    }
}
