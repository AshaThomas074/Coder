namespace Coder.Models
{
    public class JDoodleModel
    {
        public string? clientId { get; set; }
        public string? clientSecret { get; set; }
        public string? script { get; set; }
        public string? stdin { get; set; }
        public string? language { get; set; }
        public string? versionIndex { get; set; }
        public string? question { get; set; }
        public string? ErrorMessage { get; set; }
        public int? Success { get; set; }
        public string? TestOutput { get; set; }
        public int? NumberOfTestCasesPassed { get; set; }
        public int? TotalTestCases { get; set; }
    }
}
