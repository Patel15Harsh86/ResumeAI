namespace ResumeAI.Services.Matching;

public interface ISkillMatcherService
{
    List<(string Skill, bool IsMatched, decimal Score)> MatchSkills(string resumeText, List<string> requiredSkills);
}