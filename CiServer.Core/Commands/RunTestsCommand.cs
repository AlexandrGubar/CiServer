using System.Diagnostics;
using CiServer.Core.Entities;

namespace CiServer.Core.Commands;

public class RunTestsCommand : ICommand
{
    private readonly Build _build;
    private readonly string _workingDir;

    public RunTestsCommand(Build build)
    {
        _build = build;
        _workingDir = Path.Combine(Path.GetTempPath(), "CiBuilds", _build.BuildId.ToString());
    }

    public void Execute()
    {
        Console.WriteLine($"[TESTS] Running tests in {_workingDir}...");

        RunProcess("dotnet", "test", _workingDir);

        Console.WriteLine("[TESTS] All tests passed.");
    }

    private void RunProcess(string fileName, string args, string workDir)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            WorkingDirectory = workDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine($"   > {e.Data}"); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine($"   ! {e.Data}"); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Tests failed.");
        }
    }
}
