namespace Coder.Models
{
    public class LeaderBoardModel
    {
        public string? StudentName { get; set; }
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public double? Score { get; set; }
        public TimeSpan? TimeTaken { get; set; } = TimeSpan.Zero;
        public double? Percent { get; set; } = 0.0;

    }
}
