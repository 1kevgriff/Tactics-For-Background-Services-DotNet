using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
});

builder.Services.AddHangfireServer();

var host = builder.Build();

Console.WriteLine("Worker started. Processing jobs from shared database...");
Console.WriteLine("Press Ctrl+C to stop.");

host.Run();
