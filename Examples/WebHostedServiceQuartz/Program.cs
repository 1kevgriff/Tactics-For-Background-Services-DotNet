using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());

builder.Services.AddQuartz(q =>
    {
        // base Quartz scheduler, job and trigger configuration
        var cronEvery15Seconds = "0/15 * * ? * *";
        q.ScheduleJob<CleanUpTempFolderJob>(
            trigger => trigger
                .WithCronSchedule(cronEvery15Seconds, cron => cron
                    .InTimeZone(TimeZoneInfo.Utc)
                    .WithMisfireInstruction(CronTriggerMisfireInstruction.DoNothing)), // every 15 seconds
            job => job
                .WithIdentity("CleanUpTempFolderJob")
                .StoreDurably()
                .DisallowConcurrentExecution()
                .WithDescription("Cleans up the temp folder"));
    });

builder.Services.AddQuartzHostedService(options =>
    {
        // when shutting down we want jobs to complete gracefully
        options.WaitForJobsToComplete = true;
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program;
