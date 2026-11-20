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
    client.Enqueue<RetryJobs>(j => j.DefaultRetryJob(null));
    return "Enqueued job with default retry (10 retries after the first attempt, exponential backoff)";
});

app.MapGet("/custom-retry", (IBackgroundJobClient client) =>
{
    client.Enqueue<RetryJobs>(j => j.CustomRetryJob(null));
    return "Enqueued job with [AutomaticRetry(Attempts=3, DelaysInSeconds=[5,10,30])]";
});

app.MapGet("/no-retry", (IBackgroundJobClient client) =>
{
    client.Enqueue<RetryJobs>(j => j.NoRetryJob(null));
    return "Enqueued job with [AutomaticRetry(Attempts=0)] - goes straight to Failed";
});

app.Run();
