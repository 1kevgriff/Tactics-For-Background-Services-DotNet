using Tactic.WindowsService;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(configure => {
        configure.ServiceName = "Tactical Windows Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<CleanupFolderService>();
    })
    .Build();

host.Run();
