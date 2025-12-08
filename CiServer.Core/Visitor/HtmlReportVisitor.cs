using System.Text;
using CiServer.Core.Entities;

namespace CiServer.Core.Visitor;

public class HtmlReportVisitor : IVisitor
{
    private readonly StringBuilder _sb = new StringBuilder();

    public string GetHtml()
    {
        return _sb.ToString();
    }

    public void VisitProject(Project project)
    {
        _sb.AppendLine($"<h1>Project Report: {project.Name}</h1>");
        _sb.AppendLine($"<p>Repository: <a href='{project.RepoUrl}'>{project.RepoUrl}</a></p>");
        _sb.AppendLine("<hr/>");
        _sb.AppendLine("<h3>Build History:</h3>");
        _sb.AppendLine("<ul>");
    }

    public void VisitBuild(Build build)
    {
        string color = build.Status == BuildStatus.Success ? "green" :
                       build.Status == BuildStatus.Pending ? "gray" :
                       build.Status == BuildStatus.Running ? "blue" : "red";

        _sb.AppendLine("<li>");
        _sb.AppendLine($"<strong>Build ID:</strong> {build.BuildId} <br/>");
        _sb.AppendLine($"Status: <span style='color:{color}'>{build.Status}</span> <br/>");
        _sb.AppendLine($"Time: {build.StartTime}");
        _sb.AppendLine("<div style='margin-left: 20px; font-family: monospace; background: #f0f0f0; padding: 5px;'>");
    }

    public void VisitBuildLog(BuildLog log)
    {
        _sb.AppendLine($"<span>[{log.Timestamp:HH:mm:ss}] {log.Content}</span><br/>");
    }
}
