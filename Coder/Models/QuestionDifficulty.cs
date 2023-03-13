using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Coder.Models
{
    public class QuestionDifficulty
    {
        [Key]
        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; } = string.Empty;
        public ICollection<Question> Questions { get; set; }
    }
}
