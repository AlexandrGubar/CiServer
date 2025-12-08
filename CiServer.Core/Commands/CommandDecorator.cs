namespace CiServer.Core.Commands;

public abstract class CommandDecorator : ICommand
{
    protected readonly ICommand _wrappedCommand;

    public CommandDecorator(ICommand command)
    {
        _wrappedCommand = command;
    }

    public virtual void Execute()
    {
        _wrappedCommand.Execute();
    }
}