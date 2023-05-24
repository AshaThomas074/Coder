namespace Coder.Models
{
    public class SolveQuestionViewModel
    {
        public int? ContestId { get; set; }
        public int? QuestionContestId { get; set; }
        public string? JDoodleLanguageCode { get; set; }
        public string? VersionIndex { get; set; }
        public string? Error { get; set; }
        public int? Success { get; set; }
        public string? CompiledOutput { get; set; }
        public int? StudentContestFinishStatus { get; set; }
        public Question? Question { get; set; }
        public Submission? Submission { get; set; }
        public ICollection<Language>? LanguagesList { get; set; }
        public IList<QuestionViewModel>? QuestionViewModel { get; set; }
    }
}
