using System.Diagnostics;
using CiServer.Core.Entities;

namespace CiServer.Core.Commands;

public class CloneRepositoryCommand : ICommand
{
    private readonly Build _build;
    private readonly string _workingDir;

    public CloneRepositoryCommand(Build build)
    {
        _build = build;
        _workingDir = Path.Combine(Path.GetTempPath(), "CiBuilds", _build.BuildId.ToString());
    }

    public void Execute()
    {
        Console.WriteLine($"[GIT] Preparing workspace: {_workingDir}");

        if (Directory.Exists(_workingDir))
        {
            DeleteDirectory(_workingDir);
        }

        Directory.CreateDirectory(_workingDir);

        string repoUrl = _build.Project?.RepoUrl ?? throw new Exception("Repo URL is null");
        Console.WriteLine($"[GIT] Cloning {repoUrl}...");

        RunProcess("git", $"clone {repoUrl} .", _workingDir);

        Console.WriteLine("[GIT] Repository cloned successfully.");
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
            throw new Exception($"{fileName} failed with exit code {process.ExitCode}");
        }
    }

    private void DeleteDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in Directory.GetDirectories(path))
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(path, false);
    }
}