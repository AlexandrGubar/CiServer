using System.Diagnostics;
using CiServer.Core.Entities;

namespace CiServer.Core.Commands;

public class CompileCodeCommand : ICommand
{
    private readonly Build _build;
    private readonly string _workingDir;

    public CompileCodeCommand(Build build)
    {
        _build = build;
        _workingDir = Path.Combine(Path.GetTempPath(), "CiBuilds", _build.BuildId.ToString());
    }

    public void Execute()
    {
        Console.WriteLine($"[COMPILER] Building project in {_workingDir}...");

        if (!Directory.Exists(_workingDir))
        {
            throw new Exception("Source code not found. Did git clone fail?");
        }

        RunProcess("dotnet", "build", _workingDir);

        Console.WriteLine("[COMPILER] Build successful.");
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
            throw new Exception("Compilation failed.");
        }
    }
} 