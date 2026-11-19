using Hangfire;

namespace HangFireTactics.Web.Jobs;

public class SampleJobs
{
    private readonly ILogger<SampleJobs> _logger;

    public SampleJobs(ILogger<SampleJobs> logger)
    {
        _logger = logger;
    }

    public void SlowJob(int durationSeconds)
    {
        var serverName = Environment.MachineName;
        _logger.LogInformation("[{Server}] Starting slow job ({Duration}s)...", serverName, durationSeconds);
        Thread.Sleep(durationSeconds * 1000);
        _logger.LogInformation("[{Server}] Completed slow job", serverName);
    }

    public void QuickJob(string message)
    {
        var serverName = Environment.MachineName;
        _logger.LogInformation("[{Server}] Quick job: {Message}", serverName, message);
    }
}
