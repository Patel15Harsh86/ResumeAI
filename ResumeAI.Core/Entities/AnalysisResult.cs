namespace ResumeAI.Core.Entities;

public class AnalysisResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ResumeId { get; set; }
    public Guid? JobDescriptionId { get; set; }
    public decimal OverallScore { get; set; }
    public decimal SkillScore { get; set; }
    public decimal ExperienceScore { get; set; }
    public decimal FormatScore { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? Suggestions { get; set; }
    public string? AISummary { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    public Resume Resume { get; set; } = null!;
    public JobDescription? JobDescription { get; set; }
    public ICollection<SkillMatch> SkillMatches { get; set; } = new List<SkillMatch>();
}