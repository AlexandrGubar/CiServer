namespace CiServer.Core.Interfaces;
public interface IRealTimeNotifier
{
    Task SendLogAsync(string buildId, string message, string timestamp);
    Task SendStatusAsync(string buildId, string status);
}