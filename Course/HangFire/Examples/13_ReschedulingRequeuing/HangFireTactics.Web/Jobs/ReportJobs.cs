namespace HangFireTactics.Web.Jobs;

public class ReportJobs
{
    private readonly ILogger<ReportJobs> _logger;

    public ReportJobs(ILogger<ReportJobs> logger)
    {
        _logger = logger;
    }

    public void GenerateReport(string reportId)
    {
        _logger.LogInformation("Generating report {ReportId}...", reportId);
        Thread.Sleep(3000);
        _logger.LogInformation("Report {ReportId} completed!", reportId);
    }

    public void FailingJob(string jobId)
    {
        _logger.LogInformation("Starting failing job {JobId}...", jobId);
        throw new InvalidOperationException($"Job {jobId} failed intentionally for demo purposes");
    }
}
