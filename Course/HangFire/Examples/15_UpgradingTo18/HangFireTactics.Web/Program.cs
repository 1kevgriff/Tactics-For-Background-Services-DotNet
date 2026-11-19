using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => """
    Hangfire 1.8 Upgrade Demo
    
    Key settings configured:
    - SetDataCompatibilityLevel(Version_180) - enables 1.8 features
    - UseSimpleAssemblyNameTypeSerializer() - shorter type names in storage  
    - UseRecommendedSerializerSettings() - modern JSON serialization
    
    Visit /hangfire to see the dashboard
    """);

app.MapGet("/enqueue", (IBackgroundJobClient client) =>
{
    client.Enqueue<SampleJob>(j => j.Process("test data"));
    return "Enqueued job with 1.8 serialization settings";
});

app.Run();
