using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Testing Jobs Demo - see the HangFireTactics.Tests project for unit tests");

app.MapGet("/welcome/{email}", (IBackgroundJobClient client, string email) =>
{
    var userId = Guid.NewGuid().ToString("N")[..8];
    client.Enqueue<NotificationJobs>(j => j.SendWelcomeEmail(userId, email));
    return $"Enqueued welcome email for {email}";
});

app.MapGet("/order/{email}", (IBackgroundJobClient client, string email) =>
{
    var orderId = $"ORD-{Random.Shared.Next(10000, 99999)}";
    client.Enqueue<NotificationJobs>(j => j.SendOrderConfirmation(orderId, email));
    return $"Enqueued order confirmation for {orderId} to {email}";
});

app.Run();
