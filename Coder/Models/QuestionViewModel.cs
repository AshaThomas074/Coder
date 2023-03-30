namespace Coder.Models
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionHeading { get; set; } = string.Empty;
        public string Difficulty { get; set; }=string.Empty;
        public float? Score { get; set; }        
    }
}
