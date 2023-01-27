# Tactic: Windows Service

## Description
This background task tactic is useful for applications or processes that need to run on a client machine or a server.

It requires a console application. The console application is compiled into a Windows Service. The Windows Service is installed on the client machine or server. The Windows Service is configured to start automatically when the machine boots.

## Bootstrapping this Demo


### File, New Console Application
At the terminal, create a new .NET console application:

```bash
dotnet new console -o Tactic.WindowsService
```


### Packages
Add the .NET package for using a Windows Service

```bash
dotnet add package Microsoft.Extensions.Hosting.WindowsServices
```

### Program.cs
Add the following code to the Program.cs file:

```csharp
using Tactic.WindowsService;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(configure => {
        configure.ServiceName = "Tacticial Windows Service";
    })
    .Build();

host.Run();

```

### Add your Worker

Create a new file called CleanupFolderService.cs

```csharp
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
```

### Register the Worker
Back in your Program.cs file, add the following code to the `CreateHostBuilder` method:

```csharp
.ConfigureServices(services =>
    {
        services.AddHostedService<CleanupFolderService>();
    })
```

### Test the code
At the terminal, run the command:

```bash
dotnet run
```

This will start the application, create the folder C:\Temp, and start the CleanupFolderService. The CleanupFolderService will delete any files older than 1 minute in the C:\Temp folder every 15 seconds.

## Creating the Windows Service

### Publish the application
At the terminal, run the command:

```bash
dotnet publish -c Release -o publish
```

This will create a publish folder with the compiled application.

### Create the Windows Service
At the COMMAND PROMPT (as Administrator), run the command:

```bash
sc create Tactic.WindowsService binPath="{path to your application}\Tactic.WindowsService\publish\Tactic.WindowsService.exe"
```

### Start the Windows Service
At the COMMAND PROMPT (as Administrator), run the command:

```bash
sc start Tactic.WindowsService
```

### Stop the Windows Service
At the COMMAND PROMPT (as Administrator), run the command:

```bash
sc stop Tactic.WindowsService
```

### Delete the Windows Service
At the COMMAND PROMPT (as Administrator), run the command:

```bash
sc delete Tactic.WindowsService
```
