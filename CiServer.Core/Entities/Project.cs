using System.ComponentModel.DataAnnotations;
using CiServer.Core.Visitor;

namespace CiServer.Core.Entities;

public class Project : IElement
{
    [Key]
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RepoUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Build> Builds { get; set; } = new List<Build>();

    public void Accept(IVisitor visitor)
    {
        visitor.VisitProject(this);
        foreach (var build in Builds)
        {
            build.Accept(visitor);
        }
    }
}
