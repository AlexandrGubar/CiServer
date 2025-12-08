using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CiServer.Core.Entities;

public class Artifact
{
    [Key]
    public Guid ArtifactId { get; set; }
    
    [ForeignKey("Build")]
    public Guid BuildId { get; set; }
    
    public string FilePath { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Build? Build { get; set; }
}