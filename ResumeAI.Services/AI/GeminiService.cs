using Microsoft.Extensions.Configuration;
using ResumeAI.Core.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ResumeAI.Services.AI;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string Model = "gemini-2.5-flash";

    public GeminiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured.");
    }

    public async Task<GeminiAnalysisResult> AnalyzeResumeAsync(string resumeText, string? jobDescription = null)
    {
        var prompt = BuildPrompt(resumeText, jobDescription);

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.3
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            throw new InvalidOperationException("Gemini API rate limit reached. Please wait a minute and try again.");

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var textContent = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";

        return ParseAnalysisResult(textContent);
    }

    public async Task<GeminiAnalysisResult> MatchResumeToJobAsync(string resumeText, string jobDescription, List<string> requiredSkills)
    {
        var skillsList = string.Join(", ", requiredSkills);

        var prompt = new StringBuilder();
        prompt.AppendLine("You are an expert HR recruiter. Analyze how well this resume matches the job description.");
        prompt.AppendLine();
        prompt.AppendLine("RESUME:");
        prompt.AppendLine(resumeText);
        prompt.AppendLine();
        prompt.AppendLine("JOB DESCRIPTION:");
        prompt.AppendLine(jobDescription);
        prompt.AppendLine();
        prompt.AppendLine($"REQUIRED SKILLS TO CHECK: {skillsList}");
        prompt.AppendLine();
        prompt.AppendLine("Respond ONLY with valid JSON in exactly this structure:");
        prompt.AppendLine("{");
        prompt.AppendLine("  \"overallScore\": <number 0-100>,");
        prompt.AppendLine("  \"skillScore\": <number 0-100>,");
        prompt.AppendLine("  \"experienceScore\": <number 0-100>,");
        prompt.AppendLine("  \"formatScore\": <number 0-100>,");
        prompt.AppendLine("  \"strengths\": [\"strength1\", \"strength2\", \"strength3\"],");
        prompt.AppendLine("  \"weaknesses\": [\"weakness1\", \"weakness2\", \"weakness3\"],");
        prompt.AppendLine("  \"suggestions\": [\"suggestion1\", \"suggestion2\", \"suggestion3\"],");
        prompt.AppendLine("  \"summary\": \"2-3 sentence summary of how well this resume matches the job.\"");
        prompt.AppendLine("}");

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt.ToString() } } } },
            generationConfig = new { responseMimeType = "application/json", temperature = 0.3 }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            throw new InvalidOperationException("Gemini API rate limit reached. Please wait a minute and try again.");

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var textContent = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";

        return ParseAnalysisResult(textContent);
    }

    public async Task<List<GeneratedQuestion>> GenerateInterviewQuestionsAsync(string resumeText, string jobTitle, string difficulty)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"You are an expert interviewer. Generate 5 interview questions for a {difficulty} level {jobTitle} position.");
        prompt.AppendLine();
        prompt.AppendLine("Based on this resume:");
        prompt.AppendLine(resumeText);
        prompt.AppendLine();
        prompt.AppendLine("Generate a mix of Technical, Behavioral, and HR questions.");
        prompt.AppendLine("Respond ONLY with valid JSON array, no markdown:");
        prompt.AppendLine("[");
        prompt.AppendLine("  {\"questionText\": \"question here\", \"questionType\": \"Technical\"},");
        prompt.AppendLine("  {\"questionText\": \"question here\", \"questionType\": \"Behavioral\"},");
        prompt.AppendLine("  {\"questionText\": \"question here\", \"questionType\": \"HR\"},");
        prompt.AppendLine("  {\"questionText\": \"question here\", \"questionType\": \"Technical\"},");
        prompt.AppendLine("  {\"questionText\": \"question here\", \"questionType\": \"Behavioral\"}");
        prompt.AppendLine("]");

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt.ToString() } } } },
            generationConfig = new { responseMimeType = "application/json", temperature = 0.7 }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            throw new InvalidOperationException("Gemini API rate limit reached. Please wait a minute and try again.");

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var textContent = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "[]";

        using var arrayDoc = JsonDocument.Parse(textContent);
        var questions = new List<GeneratedQuestion>();

        foreach (var item in arrayDoc.RootElement.EnumerateArray())
        {
            questions.Add(new GeneratedQuestion(
                item.GetProperty("questionText").GetString() ?? "",
                item.GetProperty("questionType").GetString() ?? "Technical"
            ));
        }

        return questions;
    }

    public async Task<QuestionFeedback> EvaluateAnswerAsync(string question, string answer, string jobTitle)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"You are an expert interviewer for a {jobTitle} position.");
        prompt.AppendLine();
        prompt.AppendLine($"Question: {question}");
        prompt.AppendLine($"Candidate's Answer: {answer}");
        prompt.AppendLine();
        prompt.AppendLine("Evaluate this answer and respond ONLY with valid JSON:");
        prompt.AppendLine("{");
        prompt.AppendLine("  \"feedback\": \"detailed feedback on the answer\",");
        prompt.AppendLine("  \"score\": <number 0-100>");
        prompt.AppendLine("}");

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt.ToString() } } } },
            generationConfig = new { responseMimeType = "application/json", temperature = 0.3 }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            throw new InvalidOperationException("Gemini API rate limit reached. Please wait a minute and try again.");

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var textContent = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";

        using var feedbackDoc = JsonDocument.Parse(textContent);
        var root = feedbackDoc.RootElement;

        return new QuestionFeedback(
            root.GetProperty("feedback").GetString() ?? "",
            root.GetProperty("score").GetDecimal()
        );
    }

    private static string BuildPrompt(string resumeText, string? jobDescription)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an expert HR recruiter and resume analyst. Analyze the following resume and provide a structured assessment.");
        sb.AppendLine();
        sb.AppendLine("RESUME TEXT:");
        sb.AppendLine(resumeText);

        if (!string.IsNullOrWhiteSpace(jobDescription))
        {
            sb.AppendLine();
            sb.AppendLine("TARGET JOB DESCRIPTION:");
            sb.AppendLine(jobDescription);
            sb.AppendLine();
            sb.AppendLine("Evaluate how well this resume matches the job description above.");
        }

        sb.AppendLine();
        sb.AppendLine("Respond ONLY with valid JSON in exactly this structure, with no markdown formatting or extra text:");
        sb.AppendLine("""
        {
          "overallScore": <number 0-100>,
          "skillScore": <number 0-100>,
          "experienceScore": <number 0-100>,
          "formatScore": <number 0-100>,
          "strengths": ["strength1", "strength2", "strength3"],
          "weaknesses": ["weakness1", "weakness2", "weakness3"],
          "suggestions": ["suggestion1", "suggestion2", "suggestion3"],
          "summary": "A 2-3 sentence overall summary of the resume."
        }
        """);

        return sb.ToString();
    }

    private static GeminiAnalysisResult ParseAnalysisResult(string jsonText)
    {
        using var doc = JsonDocument.Parse(jsonText);
        var root = doc.RootElement;

        return new GeminiAnalysisResult(
            OverallScore: root.GetProperty("overallScore").GetDecimal(),
            SkillScore: root.GetProperty("skillScore").GetDecimal(),
            ExperienceScore: root.GetProperty("experienceScore").GetDecimal(),
            FormatScore: root.GetProperty("formatScore").GetDecimal(),
            Strengths: root.GetProperty("strengths").EnumerateArray().Select(x => x.GetString() ?? "").ToList(),
            Weaknesses: root.GetProperty("weaknesses").EnumerateArray().Select(x => x.GetString() ?? "").ToList(),
            Suggestions: root.GetProperty("suggestions").EnumerateArray().Select(x => x.GetString() ?? "").ToList(),
            Summary: root.GetProperty("summary").GetString() ?? ""
        );
    }
}