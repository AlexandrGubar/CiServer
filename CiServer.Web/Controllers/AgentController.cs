using CiServer.Core.Entities;
using CiServer.Core.Mediator;
using CiServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiServer.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;

    public AgentController(ApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet("work")]
    public async Task<IActionResult> GetWork()
    {
        var build = await _context.Builds
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.Status == BuildStatus.Pending);

        if (build == null)
            return NoContent();

        build.RestoreState();
        build.Start();
        await _context.SaveChangesAsync();
        Console.WriteLine($"[SERVER] Assigned job {build.BuildId}");
        return Ok(build);
    }

    [HttpPost("finish")]
    public IActionResult FinishWork([FromBody] Build result)
    {
        _mediator.Notify(this, "JobFinished", result);
        return Ok();
    }

    [HttpPost("log")]
    public IActionResult AddLog([FromBody] BuildLog log)
    {
        _mediator.Notify(this, "LogReceived", log);
        return Ok();
    }

    [HttpPost("artifact")]
    public async Task<IActionResult> UploadArtifact([FromForm] Guid buildId, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var build = await _context.Builds.FindAsync(buildId);
        if (build == null)
            return NotFound("Build not found");

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "artifacts", buildId.ToString());
        Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var artifact = new Artifact
        {
            ArtifactId = Guid.NewGuid(),
            BuildId = buildId,
            FilePath = $"/artifacts/{buildId}/{file.FileName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Artifacts.Add(artifact);
        await _context.SaveChangesAsync();
        Console.WriteLine($"[SERVER] Artifact saved: {file.FileName}");
        return Ok();
    }
}