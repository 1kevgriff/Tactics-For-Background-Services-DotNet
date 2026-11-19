using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());

builder.Services.AddQuartz(q =>
    {
        // base Quartz scheduler, job and trigger configuration
        var jobKey = new JobKey("CleanUpTempFolderJob");
        q.AddJob<CleanUpTempFolderJob>(job => job
            .WithIdentity(jobKey)
            .StoreDurably()
            .DisallowConcurrentExecution()
            .WithDescription("Cleans up the temp folder"));

        var cronEvery15Seconds = "0/15 * * ? * *";
        q.AddTrigger(trigger => trigger
            .ForJob(jobKey)
            .WithCronSchedule(cronEvery15Seconds)); // every minute
    });

builder.Services.AddQuartzHostedService(options =>
    {
        // when shutting down we want jobs to complete gracefully
        options.WaitForJobsToComplete = true;
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
