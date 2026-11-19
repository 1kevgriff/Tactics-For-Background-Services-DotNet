namespace HangFireTactics.Web.Jobs;

public class PrintJob
{
    private readonly ILogger<PrintJob> _logger;

    public PrintJob(ILogger<PrintJob> logger)
    {
        _logger = logger;
    }

    public void Print(string message)
    {
        _logger.LogInformation("Processed by worker: {Message}", message);
    }
}
