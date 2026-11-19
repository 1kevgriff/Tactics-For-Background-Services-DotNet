using Hangfire;

namespace HangFireTactics.Web.Jobs;

public class RetryJobs
{
    private readonly ILogger<RetryJobs> _logger;
    private static int _defaultAttempt = 0;
    private static int _customAttempt = 0;
    private static int _noRetryAttempt = 0;

    public RetryJobs(ILogger<RetryJobs> logger)
    {
        _logger = logger;
    }

    public void DefaultRetryJob()
    {
        _defaultAttempt++;
        _logger.LogWarning("Default retry job attempt {Attempt} (default: 10 retries)", _defaultAttempt);
        throw new Exception($"Intentional failure - attempt {_defaultAttempt}");
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 5, 10, 30 })]
    public void CustomRetryJob()
    {
        _customAttempt++;
        _logger.LogWarning("Custom retry job attempt {Attempt} (3 retries: 5s, 10s, 30s)", _customAttempt);
        throw new Exception($"Intentional failure - attempt {_customAttempt}");
    }

    [AutomaticRetry(Attempts = 0)]
    public void NoRetryJob()
    {
        _noRetryAttempt++;
        _logger.LogWarning("No retry job attempt {Attempt} (goes straight to failed)", _noRetryAttempt);
        throw new Exception("This job will not retry");
    }

    public static void ResetCounters()
    {
        _defaultAttempt = 0;
        _customAttempt = 0;
        _noRetryAttempt = 0;
    }
}
