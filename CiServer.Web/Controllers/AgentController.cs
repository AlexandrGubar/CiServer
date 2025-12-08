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
}
