namespace ResumeAI.Core.DTOs;

public record AnalyzeRequest(Guid ResumeId, Guid? JobDescriptionId);

public record AnalysisResponse(
    Guid Id,
    decimal OverallScore,
    decimal SkillScore,
    decimal ExperienceScore,
    decimal FormatScore,
    List<string> Strengths,
    List<string> Weaknesses,
    List<string> Suggestions,
    string AISummary,
    DateTime AnalyzedAt
);

public record GeminiAnalysisResult(
    decimal OverallScore,
    decimal SkillScore,
    decimal ExperienceScore,
    decimal FormatScore,
    List<string> Strengths,
    List<string> Weaknesses,
    List<string> Suggestions,
    string Summary
);