namespace CiServer.Core.Commands;

public class BuildPipeline
{
    private readonly List<ICommand> _commands = new List<ICommand>();

    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public void Run()
    {
        Console.WriteLine(">>> Pipeline started.");
        foreach (var command in _commands)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! Pipeline failed on command {command.GetType().Name}: {ex.Message}");
                throw; 
            }
        }
        Console.WriteLine(">>> Pipeline finished successfully.");
    }
}