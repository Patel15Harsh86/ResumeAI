namespace ResumeAI.Core.Entities;

public class InterviewQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "Technical";
    public string? UserAnswer { get; set; }
    public string? AIFeedback { get; set; }
    public decimal Score { get; set; }
    public int OrderIndex { get; set; }

    public InterviewSession Session { get; set; } = null!;
}