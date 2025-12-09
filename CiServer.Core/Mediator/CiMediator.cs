using CiServer.Core.Entities;
using CiServer.Core.Interfaces;

namespace CiServer.Core.Mediator;

public class CiMediator : IMediator
{
    private readonly IRepository<Build, Guid> _buildRepo;
    private readonly IRepository<BuildLog, Guid> _logRepo;
    private readonly IRealTimeNotifier _notifier;

    public CiMediator(
        IRepository<Build, Guid> buildRepo,
        IRepository<BuildLog, Guid> logRepo,
        IRealTimeNotifier notifier)
    {
        _buildRepo = buildRepo;
        _logRepo = logRepo;
        _notifier = notifier;
    }

    public void Notify(object sender, string ev, object? data = null)
    {
        HandleAsync(ev, data).Wait();
    }

    private async Task HandleAsync(string ev, object? data)
    {
        if (ev == "JobFinished" && data is Build resultBuild)
        {
            var build = await _buildRepo.GetByIdAsync(resultBuild.BuildId);
            if (build != null)
            {
                build.RestoreState();
                bool isSuccess = resultBuild.Status == BuildStatus.Success;
                build.Finish(isSuccess);
                build.EndTime = DateTime.UtcNow;
                await _buildRepo.SaveChangesAsync();
                Console.WriteLine($"[MEDIATOR] Build {build.BuildId} finalized. Status: {build.Status}");
            }

            await _notifier.SendStatusAsync(resultBuild.BuildId.ToString(), resultBuild.Status.ToString());
        }
        else if (ev == "LogReceived" && data is BuildLog log)
        {
            await _logRepo.AddAsync(log);
            await _logRepo.SaveChangesAsync();

            await _notifier.SendLogAsync(
                log.BuildId.ToString(),
                log.Content,
                log.Timestamp.ToString("HH:mm:ss")
            );

            Console.WriteLine($"[MEDIATOR] Log saved & broadcasted: {log.Content}");
        }
    }
}