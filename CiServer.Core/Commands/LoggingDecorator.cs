namespace CiServer.Core.Commands;

public class LoggingDecorator : CommandDecorator
{
    public LoggingDecorator(ICommand command) : base(command)
    {
    }

    public override void Execute()
    {
        var commandName = _wrappedCommand.GetType().Name;
        Console.WriteLine($"\n[DECORATOR] >>> START executing command: {commandName}");
        
        try 
        {
            base.Execute();
            
            Console.WriteLine($"[DECORATOR] <<< FINISHED command: {commandName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DECORATOR] !!! ERROR in command {commandName}: {ex.Message}");
            throw; 
        }
    }
}