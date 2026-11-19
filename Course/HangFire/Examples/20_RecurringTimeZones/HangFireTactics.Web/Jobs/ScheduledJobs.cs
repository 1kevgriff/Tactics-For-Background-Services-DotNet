namespace HangFireTactics.Web.Jobs;

public class ScheduledJobs
{
    private readonly ILogger<ScheduledJobs> _logger;

    public ScheduledJobs(ILogger<ScheduledJobs> logger)
    {
        _logger = logger;
    }

    public void DailyReport(string region)
    {
        _logger.LogInformation("[{Region}] Running daily report at {Time} (server time: {ServerTime})",
            region,
            DateTime.Now.ToString("HH:mm:ss"),
            TimeZoneInfo.Local.DisplayName);
    }
}
