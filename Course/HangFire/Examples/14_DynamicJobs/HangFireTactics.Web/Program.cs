using Hangfire;
using HangFireTactics.Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard();
app.MapGet("/", () => "Dynamic Jobs Demo - Manage recurring jobs per tenant at runtime");

app.MapPost("/tenants/{tenantId}/sync", (IRecurringJobManager manager, string tenantId, string? cron) =>
{
    var jobId = $"sync-{tenantId}";
    var cronExpression = cron ?? "*/5 * * * *";
    
    manager.AddOrUpdate<TenantJobs>(
        jobId,
        j => j.SyncTenantData(tenantId),
        cronExpression);
    
    return Results.Ok(new { 
        message = $"Created/updated sync job for tenant {tenantId}",
        jobId,
        cron = cronExpression
    });
});

app.MapPost("/tenants/{tenantId}/report", (IRecurringJobManager manager, string tenantId, string? cron) =>
{
    var jobId = $"report-{tenantId}";
    var cronExpression = cron ?? "0 8 * * *";
    
    manager.AddOrUpdate<TenantJobs>(
        jobId,
        j => j.GenerateTenantReport(tenantId),
        cronExpression);
    
    return Results.Ok(new {
        message = $"Created/updated report job for tenant {tenantId}",
        jobId,
        cron = cronExpression
    });
});

app.MapDelete("/tenants/{tenantId}/sync", (IRecurringJobManager manager, string tenantId) =>
{
    var jobId = $"sync-{tenantId}";
    manager.RemoveIfExists(jobId);
    return Results.Ok(new { message = $"Removed sync job for tenant {tenantId}", jobId });
});

app.MapDelete("/tenants/{tenantId}/report", (IRecurringJobManager manager, string tenantId) =>
{
    var jobId = $"report-{tenantId}";
    manager.RemoveIfExists(jobId);
    return Results.Ok(new { message = $"Removed report job for tenant {tenantId}", jobId });
});

app.MapPost("/tenants/{tenantId}/trigger-sync", (IRecurringJobManagerV2 manager, string tenantId) =>
{
    var jobId = $"sync-{tenantId}";

    // TriggerJob returns null when no recurring job has this id
    var backgroundJobId = manager.TriggerJob(jobId);
    if (backgroundJobId is null)
        return Results.NotFound(new { message = $"No sync job for tenant {tenantId}", jobId });

    return Results.Ok(new { message = $"Triggered sync job for tenant {tenantId}", jobId, backgroundJobId });
});

app.MapGet("/tenants/setup-demo", (IRecurringJobManager manager) =>
{
    var tenants = new[] { "acme-corp", "contoso", "northwind" };
    foreach (var tenant in tenants)
    {
        manager.AddOrUpdate<TenantJobs>(
            $"sync-{tenant}",
            j => j.SyncTenantData(tenant),
            "*/2 * * * *");
    }
    return Results.Ok(new { 
        message = "Set up sync jobs for demo tenants",
        tenants,
        cron = "*/2 * * * *"
    });
});

app.Run();
