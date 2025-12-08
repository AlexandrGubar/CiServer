using CiServer.Core.Entities;

namespace CiServer.Core.Mediator;

public class BuildProcessComponent : BaseComponent
{
    public void RunBuildSimulation(Build build)
    {
        Console.WriteLine($"[BuildComponent] Starting build {build.BuildId}...");
        
        Thread.Sleep(500); 
        
        build.Status = BuildStatus.Success; 

        Console.WriteLine($"[BuildComponent] Build finished.");

        _mediator.Notify(this, "BuildFinished", build);
    }
}

public class NotificationComponent : BaseComponent
{
    public void SendEmail(string message)
    {
        Console.WriteLine($"[NotificationComponent] 📧 SENDING EMAIL: {message}");
    }
}

public class DbArchiverComponent : BaseComponent
{
    public void SaveResultsToDb(Build build)
    {
        Console.WriteLine($"[DbArchiverComponent] 💾 Saving build {build.BuildId} results to Database...");
    }
}