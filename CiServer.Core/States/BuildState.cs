using CiServer.Core.Entities;

namespace CiServer.Core.States;

public abstract class BuildState
{
    protected Build _context;

    public void SetContext(Build context)
    {
        _context = context;
    }

    public abstract void StartBuild();
    public abstract void FinishBuild(bool isSuccess);
    public abstract void CancelBuild();
}