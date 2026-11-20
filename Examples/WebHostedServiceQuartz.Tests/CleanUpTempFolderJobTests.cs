using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Xunit;

public class CleanUpTempFolderJobTests
{
    private static readonly JobKey JobKey = new("CleanUpTempFolderJob");

    [Fact]
    public async Task Job_is_registered_with_a_15_second_cron_trigger()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new WebApplicationFactory<Program>();
        var scheduler = await factory.Services.GetRequiredService<ISchedulerFactory>().GetScheduler(ct);

        Assert.NotNull(await scheduler.GetJobDetail(JobKey, ct));

        var triggers = await scheduler.GetTriggersOfJob(JobKey, ct);
        var cron = Assert.IsAssignableFrom<ICronTrigger>(Assert.Single(triggers));
        Assert.Equal("0/15 * * ? * *", cron.CronExpressionString);
    }

    [Fact]
    public async Task Triggered_job_deletes_files_in_the_temp_folder()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new WebApplicationFactory<Program>();
        var scheduler = await factory.Services.GetRequiredService<ISchedulerFactory>().GetScheduler(ct);

        var tempDirectory = Path.Combine(Path.GetTempPath(), "CleanUpTempFolderService");
        Directory.CreateDirectory(tempDirectory);
        var file = Path.Combine(tempDirectory, $"{Guid.NewGuid():N}.tmp");
        await File.WriteAllTextAsync(file, "test", ct);

        var listener = new CompletionListener();
        scheduler.ListenerManager.AddJobListener(listener, [Matchers.Key(JobKey)]);

        await scheduler.TriggerJob(JobKey, null, ct);
        await listener.Executed.Task.WaitAsync(TimeSpan.FromSeconds(10), ct);

        Assert.False(File.Exists(file));
    }

    private sealed class CompletionListener : IJobListener
    {
        public TaskCompletionSource Executed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public string Name => nameof(CompletionListener);

        public ValueTask JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;

        public ValueTask JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;

        public ValueTask JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            Executed.TrySetResult();
            return ValueTask.CompletedTask;
        }
    }
}
