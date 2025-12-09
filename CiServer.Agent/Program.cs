using System.Net.Http.Json;
using CiServer.Core.Entities;
using CiServer.Core.Commands;

const string SERVER_URL = "http://localhost:5086";
using var httpClient = new HttpClient { BaseAddress = new Uri(SERVER_URL) };

Console.WriteLine($"--- CI AGENT CONNECTED TO {SERVER_URL} ---");

while (true)
{
    try
    {
        var response = await httpClient.GetAsync("/api/agent/work");

        if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            var build = await response.Content.ReadFromJsonAsync<Build>();
            if (build != null)
            {
                await ExecutePipelineAsync(build, httpClient);
            }
        }
        else
        {
            Console.Write(".");
            await Task.Delay(2000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Server unavailable: {ex.Message}");
        await Task.Delay(5000);
    }
}

static async Task ExecutePipelineAsync(Build build, HttpClient client)
{
    Console.WriteLine($"\n[JOB STARTED] BuildId: {build.BuildId}");

    var pipeline = new BuildPipeline();
    var gitCmd = new CloneRepositoryCommand(build);
    var buildCmd = new CompileCodeCommand(build);
    var testCmd = new RunTestsCommand(build);
    var artifactCmd = new ArchiveArtifactsCommand(build, client);

    var decoratedGit = new HttpReportDecorator(gitCmd, client, build.BuildId);
    var decoratedBuild = new HttpReportDecorator(buildCmd, client, build.BuildId);
    var decoratedTest = new HttpReportDecorator(testCmd, client, build.BuildId);
    var decoratedArtifact = new HttpReportDecorator(artifactCmd, client, build.BuildId);

    pipeline.AddCommand(decoratedGit);
    pipeline.AddCommand(decoratedBuild);
    pipeline.AddCommand(decoratedTest);
    pipeline.AddCommand(decoratedArtifact);

    bool success = true;
    try
    {
        pipeline.Run();
    }
    catch
    {
        success = false;
    }

    build.Status = success ? BuildStatus.Success : BuildStatus.Failed;
    await client.PostAsJsonAsync("/api/agent/finish", build);

    Console.WriteLine($"[JOB FINISHED] Status: {build.Status}\n");
}

public class HttpReportDecorator : CommandDecorator
{
    private readonly HttpClient _client;
    private readonly Guid _buildId;

    public HttpReportDecorator(ICommand command, HttpClient client, Guid buildId) : base(command)
    {
        _client = client;
        _buildId = buildId;
    }

    public override void Execute()
    {
        string name = _wrappedCommand.GetType().Name;
        SendLog($"[Start] {name}");

        try
        {
            base.Execute();
            SendLog($"[Success] {name}");
        }
        catch (Exception ex)
        {
            SendLog($"[Error] {name}: {ex.Message}");
            throw;
        }
    }

    private void SendLog(string content)
    {
        var log = new BuildLog { BuildId = _buildId, Content = content };
        _client.PostAsJsonAsync("/api/agent/log", log);
        Console.WriteLine($"   -> Sent log: {content}");
    }
}
