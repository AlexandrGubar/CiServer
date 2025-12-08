namespace CiServer.Core.Mediator;

public interface IMediator
{
    void Notify(object sender, string ev, object? data = null);
}