using CiServer.Data;
using CiServer.Core.Visitor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiServer.Web.Controllers;

public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Project(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Builds)
            .ThenInclude(b => b.Logs)
            .FirstOrDefaultAsync(p => p.ProjectId == id);

        if (project == null) return NotFound("Проєкт не знайдено");

        var visitor = new HtmlReportVisitor();
        project.Accept(visitor); 

        return Content(visitor.GetHtml(), "text/html");
    }
}