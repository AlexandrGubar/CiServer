using System.IO.Compression;
using System.Net.Http.Headers;
using CiServer.Core.Entities;

namespace CiServer.Core.Commands;

public class ArchiveArtifactsCommand : ICommand
{
    private readonly Build _build;
    private readonly HttpClient _client;
    private readonly string _workingDir;

    public ArchiveArtifactsCommand(Build build, HttpClient client)
    {
        _build = build;
        _client = client;
        _workingDir = Path.Combine(Path.GetTempPath(), "CiBuilds", _build.BuildId.ToString());
    }

    public void Execute()
    {
        Console.WriteLine("[ARTIFACTS] Searching for build output...");
        var binPath = Directory.GetDirectories(_workingDir, "bin", SearchOption.AllDirectories)
                               .FirstOrDefault();
        if (string.IsNullOrEmpty(binPath))
        {
            Console.WriteLine($"[ARTIFACTS] No 'bin' folder found in {_workingDir}. Nothing to archive.");
            return;
        }

        if (!Directory.GetFiles(binPath, "*.*", SearchOption.AllDirectories).Any())
        {
            Console.WriteLine("[ARTIFACTS] 'bin' folder exists but is empty.");
            return;
        }
        Console.WriteLine($"[ARTIFACTS] Found output at: {binPath}");

        var zipPath = Path.Combine(_workingDir, $"build_{_build.BuildId}.zip");
        if (File.Exists(zipPath)) File.Delete(zipPath);
        ZipFile.CreateFromDirectory(binPath, zipPath);
        Console.WriteLine($"[ARTIFACTS] Created archive: {zipPath}");
        UploadFile(zipPath).Wait();
    }

    private async Task UploadFile(string filePath)
    {
        using var content = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content.Add(new StringContent(_build.BuildId.ToString()), "buildId");
        content.Add(fileContent, "file", Path.GetFileName(filePath));

        try
        {
            var response = await _client.PostAsync("/api/agent/artifact", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("[ARTIFACTS] Upload successful!");
            else
                Console.WriteLine($"[ARTIFACTS] Upload failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARTIFACTS] Upload error: {ex.Message}");
        }
    }
}
