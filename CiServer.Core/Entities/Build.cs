using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CiServer.Core.States;
using CiServer.Core.Visitor;

namespace CiServer.Core.Entities;

public class Build : IElement
{
    [Key]
    public Guid BuildId { get; set; }

    [ForeignKey("Project")]
    public Guid ProjectId { get; set; }

    public string CommitHash { get; set; } = string.Empty;
    public BuildStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Project? Project { get; set; }
    public ICollection<BuildLog> Logs { get; set; } = new List<BuildLog>();
    public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();

    [NotMapped]
    private BuildState _state = default!;

    public Build()
    {
        TransitionTo(new PendingState());
    }
    public void RestoreState()
    {
        switch (Status)
        {
            case BuildStatus.Pending:
                _state = new PendingState();
                break;
            case BuildStatus.Running:
                _state = new RunningState();
                break;
            case BuildStatus.Success:
                _state = new SuccessState();
                break;
            case BuildStatus.Failed:
                _state = new FailedState();
                break;
            default:
                _state = new PendingState();
                break;
        }
        _state.SetContext(this);
    }

    public void TransitionTo(BuildState state)
    {
        _state = state;
        _state.SetContext(this);

        if (_state is PendingState)
            Status = BuildStatus.Pending;
        else if (_state is RunningState)
            Status = BuildStatus.Running;
        else if (_state is SuccessState)
            Status = BuildStatus.Success;
        else if (_state is FailedState)
            Status = BuildStatus.Failed;
    }

    public void Start() => _state.StartBuild();
    public void Finish(bool success) => _state.FinishBuild(success);
    public void Cancel() => _state.CancelBuild();

    public void Accept(IVisitor visitor)
    {
        visitor.VisitBuild(this);
        
        foreach (var log in Logs)
        {
            log.Accept(visitor);
        }

        foreach (var artifact in Artifacts)
        {
            artifact.Accept(visitor);
        }
    }
}
