using Hangfire;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage("Server=.;Database=HangfireDemo;Trusted_Connection=True;");

    var cronEvery30Seconds = "*/30 * * * * *";
    RecurringJob.AddOrUpdate<CleanupTempFolderJob>(job => job.Execute(), cronEvery30Seconds);
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/generate-now", (IBackgroundJobClient backgroundJobClient) =>
{
    backgroundJobClient.Enqueue<CleanupTempFolderJob>(job => job.Generate());
});

app.MapGet("/generate-later", (IBackgroundJobClient backgroundJobClient) =>
{
    backgroundJobClient.Schedule<CleanupTempFolderJob>(job => job.Generate(),
        TimeSpan.FromSeconds(15));
});

app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.Run();
