using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
});

builder.Services.AddHangfireServer(options =>
{
    options.ServerName = $"MyApp-{Environment.MachineName}";
    options.WorkerCount = 4;
    options.Queues = new[] { "critical", "default" };
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Server Options Demo - visit /hangfire to see server configuration");

app.MapGet("/enqueue-jobs", (IBackgroundJobClient client) =>
{
    for (var i = 0; i < 8; i++)
    {
        client.Enqueue<SampleJobs>(j => j.SlowJob(3));
    }
    return "Enqueued 8 slow jobs. With WorkerCount=4, only 4 run concurrently. Check /hangfire Servers tab.";
});

app.Run();
