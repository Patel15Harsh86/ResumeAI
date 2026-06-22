using Microsoft.AspNetCore.Mvc;
using ResumeAI.Web.Models;
using ResumeAI.Web.Services;

namespace ResumeAI.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ApiClient _api;
    public DashboardController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Login", "Account");

        var resumes = await _api.GetAsync<List<ResumeViewModel>>("/api/resumes") ?? new();
        var interviews = await _api.GetAsync<List<InterviewSessionViewModel>>("/api/interview/sessions") ?? new();

        var model = new DashboardViewModel
        {
            FullName = HttpContext.Session.GetString("FullName") ?? "User",
            TotalResumes = resumes.Count,
            TotalInterviews = interviews.Count,
            AverageScore = interviews.Any() ? Math.Round(interviews.Average(i => i.TotalScore), 1) : 0,
            RecentResumes = resumes.Take(5).ToList(),
            RecentInterviews = interviews.Take(5).ToList()
        };

        return View(model);
    }
}