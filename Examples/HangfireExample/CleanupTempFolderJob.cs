public class CleanupTempFolderJob
{
    private ILogger<CleanupTempFolderJob> _logger;

    public CleanupTempFolderJob(ILogger<CleanupTempFolderJob> logger)
    {
        _logger = logger;
    }

    public void Execute()
    {
        _logger.LogInformation("CleanupTempFolderJob started");
        string tempFolder = GetTempFolder();

        var files = Directory.GetFiles(tempFolder);
        foreach (var file in files)
        {
            try
            {
                _logger.LogInformation("Deleting file {file}", file);
                File.Delete(file);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting file {file}", file);
            }
        }

        _logger.LogInformation("CleanupTempFolderJob finished");
    }

    private static string GetTempFolder()
    {
        var tempFolder = Path.GetTempPath();
        tempFolder = Path.Combine(tempFolder, "CleanupTempFolderJob");

        if (!Directory.Exists(tempFolder))
        {
            Directory.CreateDirectory(tempFolder);
        }

        return tempFolder;
    }

    public void Generate()
    {
        _logger.LogInformation("Generate started");

        var tempFolder = GetTempFolder();

        // generate 50 random text files
        for (int i = 0; i < 50; i++)
        {
            var fileName = Path.Combine(tempFolder, $"{Guid.NewGuid()}.txt");
            File.WriteAllText(fileName, "Hello World");

            _logger.LogInformation("Generated file {file}", fileName);
        }

        _logger.LogInformation("Generate finished");
    }
}