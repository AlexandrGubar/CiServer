using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CiServer.Core.Visitor;

namespace CiServer.Core.Entities;

public class BuildLog : IElement
{
    [Key]
    public Guid LogId { get; set; }

    [ForeignKey("Build")]
    public Guid BuildId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Build? Build { get; set; }

    public void Accept(IVisitor visitor)
    {
        visitor.VisitBuildLog(this);
    }
}
