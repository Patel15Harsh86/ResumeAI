namespace ResumeAI.Services.Matching;

public class SkillMatcherService : ISkillMatcherService
{
    public List<(string Skill, bool IsMatched, decimal Score)> MatchSkills(string resumeText, List<string> requiredSkills)
    {
        var results = new List<(string, bool, decimal)>();
        var resumeLower = resumeText.ToLowerInvariant();

        foreach (var skill in requiredSkills)
        {
            var skillLower = skill.ToLowerInvariant();
            var isExactMatch = resumeLower.Contains(skillLower);

            // partial word matching — e.g. "C#" matches "c# developer"
            var words = skillLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var wordMatchCount = words.Count(w => resumeLower.Contains(w));
            var partialScore = words.Length > 0 ? (decimal)wordMatchCount / words.Length : 0;

            var score = isExactMatch ? 100m : Math.Round(partialScore * 80m, 2);
            results.Add((skill, isExactMatch || partialScore > 0.5m, score));
        }

        return results;
    }
}