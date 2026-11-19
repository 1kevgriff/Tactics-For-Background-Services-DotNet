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

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Automatic Retries Demo - visit /hangfire to watch retry behavior");

app.MapGet("/default-retry", (IBackgroundJobClient client) =>
{
    RetryJobs.ResetCounters();
    client.Enqueue<RetryJobs>(j => j.DefaultRetryJob());
    return "Enqueued job with default retry (10 attempts, exponential backoff)";
});

app.MapGet("/custom-retry", (IBackgroundJobClient client) =>
{
    RetryJobs.ResetCounters();
    client.Enqueue<RetryJobs>(j => j.CustomRetryJob());
    return "Enqueued job with [AutomaticRetry(Attempts=3, DelaysInSeconds=[5,10,30])]";
});

app.MapGet("/no-retry", (IBackgroundJobClient client) =>
{
    RetryJobs.ResetCounters();
    client.Enqueue<RetryJobs>(j => j.NoRetryJob());
    return "Enqueued job with [AutomaticRetry(Attempts=0)] - goes straight to Failed";
});

app.Run();
