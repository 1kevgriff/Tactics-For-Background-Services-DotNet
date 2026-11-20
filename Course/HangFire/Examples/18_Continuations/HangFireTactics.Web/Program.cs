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
app.MapGet("/", () => "Continuations Demo - chain jobs with ContinueJobWith");

app.MapGet("/order/{orderId}", (IBackgroundJobClient client, string orderId) =>
{
    var job1 = client.Enqueue<OrderJobs>(j => j.ValidateOrder(orderId));
    var job2 = client.ContinueJobWith<OrderJobs>(job1, j => j.ProcessPayment(orderId));
    var job3 = client.ContinueJobWith<OrderJobs>(job2, j => j.ShipOrder(orderId));
    var job4 = client.ContinueJobWith<OrderJobs>(job3, j => j.SendConfirmation(orderId));
    
    return $"Created order pipeline for {orderId}: Validate → Payment → Ship → Confirm";
});

app.Run();
