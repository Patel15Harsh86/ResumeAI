namespace ResumeAI.Core.Entities;

public class InterviewSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ResumeId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Medium";
    public string Status { get; set; } = "Active";
    public decimal TotalScore { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public User User { get; set; } = null!;
    public Resume Resume { get; set; } = null!;
    public ICollection<InterviewQuestion> Questions { get; set; } = new List<InterviewQuestion>();
}