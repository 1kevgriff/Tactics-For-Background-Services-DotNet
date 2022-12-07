using Quartz;

public class CleanUpTempFolderJob : IJob
{
    private ILogger<CleanUpTempFolderJob> _logger;
    private string _tempDirectory;

    public CleanUpTempFolderJob(ILogger<CleanUpTempFolderJob> logger)
    {
        _logger = logger;

        var tempDirectory = Path.GetTempPath();
        _tempDirectory = Path.Combine(tempDirectory, "CleanUpTempFolderService");

        _logger.LogInformation("Temp directory is {tempDirectory}", _tempDirectory);
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing job {jobKey}", context.JobDetail.Key);

        if (!Directory.Exists(_tempDirectory))
        {
            _logger.LogInformation("Creating temp directory {tempDirectory}", _tempDirectory);
            Directory.CreateDirectory(_tempDirectory);
        }

        var files = Directory.GetFiles(_tempDirectory);

        if (files.Any())
        {
            _logger.LogInformation("Found {fileCount} files in temp directory", files.Length);

            foreach (var file in files)
            {
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

        return Task.CompletedTask;
    }
}