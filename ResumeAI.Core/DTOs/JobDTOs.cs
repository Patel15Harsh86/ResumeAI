namespace ResumeAI.Core.DTOs;

public record CreateJobRequest(string Title, string Company, string Description, List<string> RequiredSkills);
public record JobResponse(Guid Id, string Title, string Company, string Description, List<string> RequiredSkills, DateTime CreatedAt);
public record JobMatchRequest(Guid ResumeId, Guid JobDescriptionId);
public record SkillMatchResult(string SkillName, bool IsMatched, decimal MatchScore, string Category);
public record JobMatchResponse(
    Guid AnalysisId,
    string JobTitle,
    string Company,
    decimal OverallScore,
    decimal SkillScore,
    decimal ExperienceScore,
    decimal FormatScore,
    List<SkillMatchResult> SkillMatches,
    List<string> Strengths,
    List<string> Weaknesses,
    List<string> Suggestions,
    string AISummary
);