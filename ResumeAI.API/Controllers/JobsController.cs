using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.DTOs;
using ResumeAI.Core.Entities;
using ResumeAI.Infrastructure.Data;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;

    public JobsController(AppDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<ActionResult<JobResponse>> Create(CreateJobRequest request)
    {
        var job = new JobDescription
        {
            UserId = CurrentUserId,
            Title = request.Title,
            Company = request.Company,
            Description = request.Description,
            RequiredSkills = JsonSerializer.Serialize(request.RequiredSkills)
        };

        _db.JobDescriptions.Add(job);
        await _db.SaveChangesAsync();

        return Ok(ToResponse(job));
    }

    [HttpGet]
    public async Task<ActionResult<List<JobResponse>>> GetAll()
    {
        var jobs = await _db.JobDescriptions
            .Where(j => j.UserId == CurrentUserId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return Ok(jobs.Select(ToResponse).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobResponse>> GetById(Guid id)
    {
        var job = await _db.JobDescriptions.FirstOrDefaultAsync(j => j.Id == id && j.UserId == CurrentUserId);
        if (job is null) return NotFound();
        return Ok(ToResponse(job));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var job = await _db.JobDescriptions.FirstOrDefaultAsync(j => j.Id == id && j.UserId == CurrentUserId);
        if (job is null) return NotFound();
        _db.JobDescriptions.Remove(job);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static JobResponse ToResponse(JobDescription job) => new(
        job.Id, job.Title, job.Company, job.Description,
        JsonSerializer.Deserialize<List<string>>(job.RequiredSkills ?? "[]") ?? new(),
        job.CreatedAt);
}