using Tactic.WindowsService;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(configure => {
        configure.ServiceName = "Tacticial Windows Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
