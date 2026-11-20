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

builder.Services.AddHangfireServer(options => options.WorkerCount = 4);

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Disable Concurrent Execution Demo - 4 workers configured");

app.MapGet("/concurrent", (IBackgroundJobClient client) =>
{
    for (var i = 1; i <= 4; i++)
        client.Enqueue<ConcurrencyJobs>(j => j.ConcurrentJob($"job-{i}"));
    return "Enqueued 4 concurrent jobs - all run in parallel";
});

app.MapGet("/singleton", (IBackgroundJobClient client) =>
{
    for (var i = 1; i <= 4; i++)
        client.Enqueue<ConcurrencyJobs>(j => j.SingletonJob($"job-{i}"));
    return "Enqueued 4 singleton jobs - only one runs at a time";
});

app.MapGet("/per-tenant", (IBackgroundJobClient client) =>
{
    client.Enqueue<ConcurrencyJobs>(j => j.PerTenantJob("acme", "task-1"));
    client.Enqueue<ConcurrencyJobs>(j => j.PerTenantJob("acme", "task-2"));
    client.Enqueue<ConcurrencyJobs>(j => j.PerTenantJob("contoso", "task-1"));
    client.Enqueue<ConcurrencyJobs>(j => j.PerTenantJob("contoso", "task-2"));
    return "Enqueued 4 jobs using 'tenant:{0}' resource - acme jobs run serially, contoso jobs run serially, but acme and contoso can run in parallel";
});

app.Run();
