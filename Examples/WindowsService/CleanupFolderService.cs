namespace Tactic.WindowsService;

public class CleanupFolderService : BackgroundService
{
    private const string FolderToCheck = "C:\\Temp";
    private const int DelayBetweenChecks = 1000 * 15; // 15 seconds

    private readonly ILogger<CleanupFolderService> _logger;

    public CleanupFolderService(ILogger<CleanupFolderService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // create FolderToCheck if it doesn't exist
        if (!Directory.Exists(FolderToCheck))
        {
            Directory.CreateDirectory(FolderToCheck);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking folder {FolderToCheck}", FolderToCheck);

            // delete any files older than 1 minute in FolderToCheck
            var files = Directory.GetFiles(FolderToCheck, "*.*", SearchOption.AllDirectories)
                .Where(f => File.GetLastWriteTime(f) < DateTime.Now.AddMinutes(-1));

            if (files.Any())
                _logger.LogInformation("Found {FileCount} files to delete", files.Count());

            foreach (var file in files)
            {
                _logger.LogInformation("Deleting file {File}", file);
                File.Delete(file);
            }

            await Task.Delay(DelayBetweenChecks, stoppingToken);
        }

        _logger.LogInformation("Shutting down worker");
    }
}
