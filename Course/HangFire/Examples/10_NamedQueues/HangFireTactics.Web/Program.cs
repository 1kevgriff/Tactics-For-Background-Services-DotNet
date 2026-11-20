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
    options.Queues = new[] { "critical", "default", "low-priority" };
});

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Named Queues Demo - visit /hangfire to see the dashboard");

app.MapGet("/enqueue-critical", (IBackgroundJobClient client) =>
{
    var orderId = Guid.NewGuid().ToString("N")[..8];
    client.Enqueue<QueuedJobs>(j => j.ProcessCriticalOrder(orderId));
    return $"Enqueued critical order {orderId} (uses [Queue(\"critical\")] attribute)";
});

app.MapGet("/enqueue-standard", (IBackgroundJobClient client) =>
{
    var orderId = Guid.NewGuid().ToString("N")[..8];
    client.Enqueue<QueuedJobs>(j => j.ProcessStandardOrder(orderId));
    return $"Enqueued standard order {orderId} (uses [Queue(\"default\")] attribute)";
});

app.MapGet("/enqueue-marketing", (IBackgroundJobClient client) =>
{
    var email = $"user-{Random.Shared.Next(1000)}@example.com";
    client.Enqueue<QueuedJobs>(j => j.SendMarketingEmail(email));
    return $"Enqueued marketing email to {email} (uses [Queue(\"low-priority\")] attribute)";
});

app.MapGet("/enqueue-to-queue/{queue}", (IBackgroundJobClient client, string queue) =>
{
    var message = $"Message at {DateTime.Now:HH:mm:ss}";
    client.Enqueue(queue, () => Console.WriteLine($"[{queue}] {message}"));
    return $"Enqueued job to queue '{queue}' using Enqueue(queue, ...) overload";
});

app.MapGet("/schedule-to-queue/{queue}", (IBackgroundJobClient client, string queue) =>
{
    var message = $"Scheduled message at {DateTime.Now:HH:mm:ss}";
    client.Schedule(queue, () => Console.WriteLine($"[{queue}] {message}"), TimeSpan.FromSeconds(10));
    return $"Scheduled job to queue '{queue}' in 10 seconds using Schedule(queue, ...) overload";
});

app.Run();
