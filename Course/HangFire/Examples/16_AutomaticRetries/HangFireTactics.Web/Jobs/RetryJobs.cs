using Hangfire;
using Hangfire.Server;

namespace HangFireTactics.Web.Jobs;

public class RetryJobs
{
    private readonly ILogger<RetryJobs> _logger;

    public RetryJobs(ILogger<RetryJobs> logger)
    {
        _logger = logger;
    }

    // Hangfire stores RetryCount per job, so each job reports its own attempt number
    private static int Attempt(PerformContext? context) =>
        (context?.GetJobParameter<int>("RetryCount") ?? 0) + 1;

    public void DefaultRetryJob(PerformContext? context)
    {
        var jobId = context?.BackgroundJob.Id;
        _logger.LogWarning("Default retry job {JobId} attempt {Attempt} (default: 10 retries)", jobId, Attempt(context));
        throw new Exception($"Intentional failure - job {jobId}, attempt {Attempt(context)}");
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 5, 10, 30 })]
    public void CustomRetryJob(PerformContext? context)
    {
        var jobId = context?.BackgroundJob.Id;
        _logger.LogWarning("Custom retry job {JobId} attempt {Attempt} (3 retries: 5s, 10s, 30s)", jobId, Attempt(context));
        throw new Exception($"Intentional failure - job {jobId}, attempt {Attempt(context)}");
    }

    [AutomaticRetry(Attempts = 0)]
    public void NoRetryJob(PerformContext? context)
    {
        var jobId = context?.BackgroundJob.Id;
        _logger.LogWarning("No retry job {JobId} attempt {Attempt} (goes straight to failed)", jobId, Attempt(context));
        throw new Exception($"Job {jobId} will not retry");
    }
}
