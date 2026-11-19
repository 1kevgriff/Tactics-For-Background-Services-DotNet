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
    options.ShutdownTimeout = TimeSpan.FromSeconds(15);
    options.StopTimeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => """
    Cancellation & Shutdown Demo
    
    Server configured with:
    - ShutdownTimeout: 15s (time to wait for jobs to finish)
    - StopTimeout: 5s (time to wait for graceful stop)
    
    Try: Start a long job, then press Ctrl+C to see cancellation in action
    """);

app.MapGet("/long-job", (IBackgroundJobClient client) =>
{
    client.Enqueue<LongRunningJobs>(j => j.LongJobWithCancellation(CancellationToken.None));
    return "Started 30-step job with CancellationToken - stops gracefully on shutdown";
});

app.MapGet("/async-job", (IBackgroundJobClient client) =>
{
    client.Enqueue<LongRunningJobs>(j => j.AsyncLongJob(CancellationToken.None));
    return "Started async 30-step job with CancellationToken - stops gracefully on shutdown";
});

app.MapGet("/bad-job", (IBackgroundJobClient client) =>
{
    client.Enqueue<LongRunningJobs>(j => j.JobIgnoringCancellation());
    return "Started job that ignores cancellation - will be forcefully terminated on shutdown";
});

app.Run();
