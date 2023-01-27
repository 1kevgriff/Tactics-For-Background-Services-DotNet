public class CleanUpTempFolderService : BackgroundService
{
    private readonly ILogger<CleanUpTempFolderService> _logger;
    private int Delay = 15000;

    public CleanUpTempFolderService(ILogger<CleanUpTempFolderService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CleanUpTempFolderService is starting.");

        var tempDirectory = Path.GetTempPath();
        tempDirectory = Path.Combine(tempDirectory, "CleanUpTempFolderService");

        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        _logger.LogInformation("Temp directory is located at {tempDirectory}", tempDirectory);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("It's currently {time} and CleanUpTempFolderService task doing background work.", DateTimeOffset.Now);

            // check tempDirectory for files
            var files = Directory.GetFiles(tempDirectory);

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

            await Task.Delay(Delay, stoppingToken);
        }
    }
}