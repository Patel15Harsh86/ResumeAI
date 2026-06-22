using Microsoft.AspNetCore.Mvc;
using ResumeAI.Web.Models;
using ResumeAI.Web.Services;

namespace ResumeAI.Web.Controllers;

public class JobController : Controller
{
    private readonly ApiClient _api;
    public JobController(ApiClient api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var jobs = await _api.GetAsync<List<JobViewModel>>("/api/jobs") ?? new();
        var resumes = await _api.GetAsync<List<ResumeViewModel>>("/api/resumes") ?? new();
        ViewBag.Resumes = resumes;
        return View(jobs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        return View(new CreateJobViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateJobViewModel model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var skills = model.RequiredSkillsText
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToList();

        var result = await _api.PostAsync<JobViewModel>("/api/jobs", new
        {
            model.Title,
            model.Company,
            model.Description,
            RequiredSkills = skills
        });

        if (result == null)
        {
            ModelState.AddModelError("", "Failed to create job. Please try again.");
            return View(model);
        }

        TempData["Success"] = "Job description created successfully!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Match(Guid jobId, Guid resumeId)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.PostAsync<JobMatchViewModel>("/api/analysis/match-job",
            new { ResumeId = resumeId, JobDescriptionId = jobId });

        if (result == null)
        {
            TempData["Error"] = "Matching failed. Please try again.";
            return RedirectToAction("Index");
        }

        return View(result);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        await _api.DeleteAsync($"/api/jobs/{id}");
        TempData["Success"] = "Job deleted successfully.";
        return RedirectToAction("Index");
    }
}