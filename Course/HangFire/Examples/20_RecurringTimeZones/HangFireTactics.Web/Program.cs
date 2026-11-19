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
app.MapGet("/", () => $"""
    Recurring Time Zones Demo
    
    Server time zone: {TimeZoneInfo.Local.DisplayName}
    Current UTC: {DateTime.UtcNow:HH:mm:ss}
    
    Important: Azure/cloud hosts typically run in UTC.
    Without TimeZone option, "0 9 * * *" runs at 9 AM UTC, not local time!
    """);

app.MapGet("/setup", (IRecurringJobManager manager) =>
{
    var eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
    var pacific = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
    var utc = TimeZoneInfo.Utc;

    manager.AddOrUpdate<ScheduledJobs>(
        "report-utc",
        j => j.DailyReport("UTC"),
        "* * * * *",
        new RecurringJobOptions { TimeZone = utc });

    manager.AddOrUpdate<ScheduledJobs>(
        "report-eastern",
        j => j.DailyReport("Eastern"),
        "* * * * *",
        new RecurringJobOptions { TimeZone = eastern });

    manager.AddOrUpdate<ScheduledJobs>(
        "report-pacific",
        j => j.DailyReport("Pacific"),
        "* * * * *",
        new RecurringJobOptions { TimeZone = pacific });

    return "Created 3 recurring jobs with different time zones (running every minute for demo)";
});

app.Run();
