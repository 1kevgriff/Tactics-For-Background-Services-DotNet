using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
});

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Separate Worker Demo - Web app enqueues jobs, Worker process executes them");

app.MapGet("/enqueue", (IBackgroundJobClient client) =>
{
    var message = $"Job created at {DateTime.Now:HH:mm:ss}";
    client.Enqueue<PrintJob>(j => j.Print(message));
    return $"Enqueued: {message}. Start the Worker project to process it.";
});

app.Run();
