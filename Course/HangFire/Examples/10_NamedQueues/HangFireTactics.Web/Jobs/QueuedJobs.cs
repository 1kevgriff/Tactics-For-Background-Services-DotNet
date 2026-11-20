using Hangfire;

namespace HangFireTactics.Web.Jobs;

public class QueuedJobs
{
    private readonly ILogger<QueuedJobs> _logger;

    public QueuedJobs(ILogger<QueuedJobs> logger)
    {
        _logger = logger;
    }

    [Queue("critical")]
    public void ProcessCriticalOrder(string orderId)
    {
        _logger.LogInformation("Processing CRITICAL order {OrderId} on queue 'critical'", orderId);
        Thread.Sleep(2000);
        _logger.LogInformation("Completed CRITICAL order {OrderId}", orderId);
    }

    [Queue("default")]
    public void ProcessStandardOrder(string orderId)
    {
        _logger.LogInformation("Processing standard order {OrderId} on queue 'default'", orderId);
        Thread.Sleep(2000);
        _logger.LogInformation("Completed standard order {OrderId}", orderId);
    }

    [Queue("low-priority")]
    public void SendMarketingEmail(string email)
    {
        _logger.LogInformation("Sending marketing email to {Email} on queue 'low-priority'", email);
        Thread.Sleep(1000);
        _logger.LogInformation("Sent marketing email to {Email}", email);
    }

    public void ProcessWithoutQueueAttribute(string message)
    {
        _logger.LogInformation("Processing job without [Queue] attribute: {Message}", message);
        Thread.Sleep(1000);
    }
}
