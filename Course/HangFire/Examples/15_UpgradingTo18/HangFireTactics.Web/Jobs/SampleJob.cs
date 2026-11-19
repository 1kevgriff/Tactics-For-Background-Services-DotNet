namespace HangFireTactics.Web.Jobs;

public class SampleJob
{
    private readonly ILogger<SampleJob> _logger;

    public SampleJob(ILogger<SampleJob> logger)
    {
        _logger = logger;
    }

    public void Process(string data)
    {
        _logger.LogInformation("Processing: {Data}", data);
    }
}
