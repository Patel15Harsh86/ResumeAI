using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.DTOs;
using ResumeAI.Core.Entities;
using ResumeAI.Infrastructure.Data;
using ResumeAI.Services.Parsing;
using ResumeAI.Services.Storage;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/resumes")]
[Authorize]
public class ResumesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private readonly IResumeParserService _parser;

    private static readonly string[] AllowedExtensions = { ".pdf", ".docx" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public ResumesController(AppDbContext db, IFileStorageService fileStorage, IResumeParserService parser)
    {
        _db = db;
        _fileStorage = fileStorage;
        _parser = parser;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost("upload")]
    public async Task<ActionResult<ResumeUploadResponse>> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest("Only PDF and DOCX files are allowed.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest("File size must not exceed 5 MB.");

        var userId = CurrentUserId;

        await using var stream = file.OpenReadStream();
        var savedPath = await _fileStorage.SaveFileAsync(stream, file.FileName, userId);

        var fileType = extension.TrimStart('.').ToUpperInvariant();

        var resume = new Resume
        {
            UserId = userId,
            FileName = file.FileName,
            FilePath = savedPath,
            FileType = fileType,
            Status = "Pending"
        };

        _db.Resumes.Add(resume);
        await _db.SaveChangesAsync();

        try
        {
            var extractedText = await _parser.ExtractTextAsync(savedPath, fileType);
            resume.ExtractedText = extractedText;
            resume.Status = "Parsed";
        }
        catch
        {
            resume.Status = "Failed";
        }

        await _db.SaveChangesAsync();

        return Ok(new ResumeUploadResponse(resume.Id, resume.FileName, resume.FileType, resume.Status, resume.UploadedAt));
    }

    [HttpGet]
    public async Task<ActionResult<List<ResumeListItem>>> GetMyResumes()
    {
        var resumes = await _db.Resumes
            .Where(r => r.UserId == CurrentUserId)
            .OrderByDescending(r => r.UploadedAt)
            .Select(r => new ResumeListItem(r.Id, r.FileName, r.FileType, r.Status, r.UploadedAt))
            .ToListAsync();

        return Ok(resumes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResumeDetail>> GetById(Guid id)
    {
        var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (resume is null) return NotFound();

        return Ok(new ResumeDetail(resume.Id, resume.FileName, resume.FileType, resume.Status, resume.ExtractedText, resume.UploadedAt));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (resume is null) return NotFound();

        _fileStorage.DeleteFile(resume.FilePath);
        _db.Resumes.Remove(resume);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}