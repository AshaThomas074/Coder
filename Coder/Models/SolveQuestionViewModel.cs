namespace Coder.Models
{
    public class SolveQuestionViewModel
    {
        public int? ContestId { get; set; }
        public int? QuestionContestId { get; set; }
        public Question? Question { get; set; }
        public Submission? Submission { get; set; }
        public ICollection<Language>? LanguagesList { get; set; }
        public IList<QuestionViewModel>? QuestionViewModel { get; set; }
    }
}
