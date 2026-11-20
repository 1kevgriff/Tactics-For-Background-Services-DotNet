using Quartz;

public class CleanUpTempFolderJob : IJob
{
    private readonly ILogger<CleanUpTempFolderJob> _logger;
    private readonly string _tempDirectory;

    public CleanUpTempFolderJob(ILogger<CleanUpTempFolderJob> logger)
    {
        _logger = logger;
        _tempDirectory = Path.Combine(Path.GetTempPath(), "CleanUpTempFolderService");

        // Quartz creates a new job instance for every fire, so keep this quiet
        _logger.LogDebug("Temp directory is {tempDirectory}", _tempDirectory);
    }

    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Executing job {jobKey} (scheduled {scheduledFireTimeUtc}, fired {fireTimeUtc})",
            context.JobDetail.Key, context.ScheduledFireTimeUtc, context.FireTimeUtc);

        // CreateDirectory does nothing if the directory already exists
        Directory.CreateDirectory(_tempDirectory);

        var files = Directory.GetFiles(_tempDirectory);

        if (files.Length > 0)
        {
            _logger.LogInformation("Found {fileCount} files in temp directory", files.Length);

            foreach (var file in files)
            {
                // stop early if the scheduler interrupts the job or shuts down
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted file {file}", file);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete file {file}", file);
                }
            }
        }

        _logger.LogInformation("Finished executing job {jobKey}", context.JobDetail.Key);

        return ValueTask.CompletedTask;
    }
}
