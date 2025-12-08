using CiServer.Core.Entities;
using CiServer.Core.Visitor;
using Microsoft.AspNetCore.Mvc;

namespace CiServer.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestVisitorController : ControllerBase
{
    [HttpGet]
    public ContentResult GetReport()
    {
        var project = new Project 
        { 
            Name = "My Awesome App", 
            RepoUrl = "https://github.com/me/app" 
        };

        var build1 = new Build 
        { 
            BuildId = Guid.NewGuid(), 
            Status = BuildStatus.Success, 
            StartTime = DateTime.UtcNow.AddHours(-1) 
        };
        build1.Logs.Add(new BuildLog { Content = "Cloning repo..." });
        build1.Logs.Add(new BuildLog { Content = "Compiling..." });
        build1.Logs.Add(new BuildLog { Content = "Success!" });

        var build2 = new Build 
        { 
            BuildId = Guid.NewGuid(), 
            Status = BuildStatus.Failed, 
            StartTime = DateTime.UtcNow 
        };
        build2.Logs.Add(new BuildLog { Content = "Cloning repo..." });
        build2.Logs.Add(new BuildLog { Content = "Error: Syntax exception on line 42" });

        project.Builds.Add(build1);
        project.Builds.Add(build2);

        var visitor = new HtmlReportVisitor();

        project.Accept(visitor);

        string html = visitor.GetHtml();

        return base.Content(html, "text/html");
    }
}