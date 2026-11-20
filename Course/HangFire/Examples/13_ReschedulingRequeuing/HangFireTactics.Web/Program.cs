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
app.MapGet("/", () => "Rescheduling & Requeuing Demo - visit /hangfire to see the dashboard");

app.MapGet("/schedule", (IBackgroundJobClient client) =>
{
    var reportId = Guid.NewGuid().ToString("N")[..8];
    var jobId = client.Schedule<ReportJobs>(
        j => j.GenerateReport(reportId),
        TimeSpan.FromMinutes(5));
    return $"Scheduled report {reportId} in 5 minutes. Job ID: {jobId}. Use /reschedule/{jobId} to change the time.";
});

app.MapGet("/reschedule/{jobId}", (string jobId) =>
{
    var success = BackgroundJob.Reschedule(jobId, TimeSpan.FromSeconds(10));
    return success
        ? $"Rescheduled job {jobId} to run in 10 seconds"
        : $"Failed to reschedule job {jobId} - job may not exist or already processed";
});

app.MapGet("/enqueue-failing", (IBackgroundJobClient client) =>
{
    var id = Guid.NewGuid().ToString("N")[..8];
    var jobId = client.Enqueue<ReportJobs>(j => j.FailingJob(id));
    return $"Enqueued failing job {id}. Job ID: {jobId}. After it fails, use /requeue/{jobId} to retry it.";
});

app.MapGet("/requeue/{jobId}", (string jobId) =>
{
    var success = BackgroundJob.Requeue(jobId);
    return success
        ? $"Requeued job {jobId} - it will run again"
        : $"Failed to requeue job {jobId} - job may not exist";
});

app.MapGet("/delete/{jobId}", (string jobId) =>
{
    var success = BackgroundJob.Delete(jobId);
    return success
        ? $"Deleted job {jobId}"
        : $"Failed to delete job {jobId} - job may not exist or already processed";
});

app.MapGet("/enqueue-many", (IBackgroundJobClient client) =>
{
    var jobIds = new List<string>();
    for (var i = 0; i < 5; i++)
    {
        var reportId = $"batch-{i + 1}";
        var jobId = client.Schedule<ReportJobs>(
            j => j.GenerateReport(reportId),
            TimeSpan.FromMinutes(10));
        jobIds.Add(jobId);
    }
    return $"Scheduled 5 jobs for 10 minutes from now. Job IDs: {string.Join(", ", jobIds)}";
});

app.Run();
