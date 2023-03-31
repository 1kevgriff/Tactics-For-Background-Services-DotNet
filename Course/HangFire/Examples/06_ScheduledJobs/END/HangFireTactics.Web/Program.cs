using System.Text.Json;
using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

// configure logging to console
builder.Logging.AddConsole();

builder.Services.AddHttpClient();

builder.Services.AddHangfire(config =>
{
    // use DefaultConnection connection string
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Hello World!");

app.MapGet("/pull", (IBackgroundJobClient client) =>
{
    var url = "https://consultwithgriff.com/rss.xml";
    var directory = $"c:\\rss";
    var filename = "consultwithgriff.json";
    var tempPath = Path.Combine(directory, filename);

    // TODO: background work
    client.Enqueue<WebPuller>(p=> p.GetRssItemUrlsAsync(url, tempPath));
});

app.MapGet("/sync", (IBackgroundJobClient client) => {

    var directory = $"c:\\rss";
    var filename = "consultwithgriff.json";

    var path = Path.Combine(directory, filename);
    var json = File.ReadAllText(path);
    var rssItemUrls = JsonSerializer.Deserialize<List<string>>(json);

    var outputPath = Path.Combine(directory, "output");
    if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

    if (rssItemUrls == null || rssItemUrls.Count == 0) return;
    var delayInSeconds = 5;
    
    foreach (var url in rssItemUrls)
    {
        var u = new Uri(url);
        var stub = u.Segments.Last();
        // trim trailing slash, if any and add .html
        if (stub.EndsWith("/")) stub = stub.Substring(0, stub.Length - 1);
        stub += ".html";

        var filePath = Path.Combine(outputPath, stub);

        var dt = DateTimeOffset.UtcNow.AddSeconds(delayInSeconds);

        client.Schedule<WebPuller>(p => p.DownloadFileFromUrl(url, filePath), 
            TimeSpan.FromSeconds(delayInSeconds));

        delayInSeconds += 5;
    }
});

app.Run();
