namespace HangFireTactics.Web.Jobs;

public class TenantJobs
{
    private readonly ILogger<TenantJobs> _logger;

    public TenantJobs(ILogger<TenantJobs> logger)
    {
        _logger = logger;
    }

    public void SyncTenantData(string tenantId)
    {
        _logger.LogInformation("[{TenantId}] Starting data sync...", tenantId);
        Thread.Sleep(2000);
        _logger.LogInformation("[{TenantId}] Data sync completed!", tenantId);
    }

    public void GenerateTenantReport(string tenantId)
    {
        _logger.LogInformation("[{TenantId}] Generating daily report...", tenantId);
        Thread.Sleep(1000);
        _logger.LogInformation("[{TenantId}] Report generated!", tenantId);
    }
}
