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

app.Run();
