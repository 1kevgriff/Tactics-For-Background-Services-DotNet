using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());

builder.Services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();

        // base Quartz scheduler, job and trigger configuration
        var jobKey = new JobKey("CleanUpTempFolderJob");
        q.AddJob<CleanUpTempFolderJob>(jobKey, job => job
            .StoreDurably()
            .DisallowConcurrentExecution()
            .WithDescription("Cleans up the temp folder"));

        var cronEvery30Seconds = "0/15 * * ? * *";
        q.AddTrigger(trigger => trigger
            .ForJob(jobKey)
            .WithCronSchedule(cronEvery30Seconds)); // every minute

    });

builder.Services.AddQuartzServer(options =>
    {
        // when shutting down we want jobs to complete gracefully
        options.WaitForJobsToComplete = true;
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
