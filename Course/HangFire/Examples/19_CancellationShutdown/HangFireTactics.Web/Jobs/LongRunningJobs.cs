using Hangfire;

namespace HangFireTactics.Web.Jobs;

public class LongRunningJobs
{
    private readonly ILogger<LongRunningJobs> _logger;

    public LongRunningJobs(ILogger<LongRunningJobs> logger)
    {
        _logger = logger;
    }

    public void LongJobWithCancellation(IJobCancellationToken cancellationToken)
    {
        for (var i = 1; i <= 30; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Processing step {Step}/30...", i);
            Thread.Sleep(1000);
        }
        _logger.LogInformation("Job completed all 30 steps!");
    }

    public async Task AsyncLongJob(CancellationToken cancellationToken)
    {
        for (var i = 1; i <= 30; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Async step {Step}/30...", i);
            await Task.Delay(1000, cancellationToken);
        }
        _logger.LogInformation("Async job completed!");
    }

    public void JobIgnoringCancellation()
    {
        for (var i = 1; i <= 30; i++)
        {
            _logger.LogInformation("Ignoring cancellation - step {Step}/30...", i);
            Thread.Sleep(1000);
        }
        _logger.LogInformation("Job finished (never checked cancellation)");
    }
}
