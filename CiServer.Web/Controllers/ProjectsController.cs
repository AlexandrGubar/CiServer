using CiServer.Data;
using CiServer.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiServer.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
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
        return RedirectToAction(nameof(Index));
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
        Console.WriteLine($"[DEBUG DELETE] Отримано запит на видалення ID: {projectId}");
        var project = await _context.Projects.FindAsync(projectId);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            Console.WriteLine("[DEBUG DELETE] Проєкт успішно видалено з БД");
        }
        else
        {
            Console.WriteLine("[DEBUG DELETE] Проєкт не знайдено в БД");
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