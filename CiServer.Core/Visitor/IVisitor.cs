using CiServer.Core.Entities;

namespace CiServer.Core.Visitor;

public interface IVisitor
{
    void VisitProject(Project project);
    void VisitBuild(Build build);
    void VisitBuildLog(BuildLog log);
    void VisitArtifact(Artifact artifact);
}