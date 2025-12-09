using CiServer.Data;
using CiServer.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CiServer.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProjectsController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects.ToListAsync();
        return View(projects);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Project project)
    {
        if (ModelState.IsValid)
        {
            project.ProjectId = Guid.NewGuid();
            project.CreatedAt = DateTime.UtcNow;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Builds)
            .FirstOrDefaultAsync(m => m.ProjectId == id);
        if (project == null)
            return NotFound();
        return View(project);
    }

    [HttpGet]
    public async Task<IActionResult> StartBuild(Guid id)
    {
        var build = new Build
        {
            BuildId = Guid.NewGuid(),
            ProjectId = id,
            Status = BuildStatus.Pending,
            StartTime = DateTime.UtcNow
        };
        _context.Builds.Add(build);
        await _context.SaveChangesAsync();
        return RedirectToAction("BuildDetails", new { id = build.BuildId });
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var project = await _context.Projects
            .FirstOrDefaultAsync(m => m.ProjectId == id);
        if (project == null)
        {
            return NotFound();
        }
        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Builds)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        if (project != null)
        {
            foreach (var build in project.Builds)
            {
                var buildArtifactsPath = Path.Combine(_env.WebRootPath, "artifacts", build.BuildId.ToString());
                if (Directory.Exists(buildArtifactsPath))
                {
                    try
                    {
                        Directory.Delete(buildArtifactsPath, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Не вдалося видалити папку {buildArtifactsPath}: {ex.Message}");
                    }
                }
            }
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> BuildDetails(Guid id)
    {
        var build = await _context.Builds
            .Include(b => b.Logs)
            .Include(b => b.Artifacts)
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.BuildId == id);

        if (build == null) return NotFound();

        return View(build);
    }
}