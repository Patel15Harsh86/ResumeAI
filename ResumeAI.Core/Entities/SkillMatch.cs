namespace ResumeAI.Core.Entities;

public class SkillMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AnalysisId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public bool IsMatched { get; set; }
    public decimal MatchScore { get; set; }
    public string Category { get; set; } = "Technical";

    public AnalysisResult Analysis { get; set; } = null!;
}