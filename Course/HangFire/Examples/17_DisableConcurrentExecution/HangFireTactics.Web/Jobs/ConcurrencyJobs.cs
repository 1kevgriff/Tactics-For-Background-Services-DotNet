using Hangfire;

namespace HangFireTactics.Web.Jobs;

public class ConcurrencyJobs
{
    private readonly ILogger<ConcurrencyJobs> _logger;

    public ConcurrencyJobs(ILogger<ConcurrencyJobs> logger)
    {
        _logger = logger;
    }

    public void ConcurrentJob(string id)
    {
        _logger.LogInformation("[{Id}] Starting concurrent job...", id);
        Thread.Sleep(5000);
        _logger.LogInformation("[{Id}] Finished concurrent job", id);
    }

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public void SingletonJob(string id)
    {
        _logger.LogInformation("[{Id}] Starting singleton job (others wait)...", id);
        Thread.Sleep(5000);
        _logger.LogInformation("[{Id}] Finished singleton job", id);
    }

    [DisableConcurrentExecution("tenant:{0}", 60)]
    public void PerTenantJob(string tenantId, string taskId)
    {
        _logger.LogInformation("[{Tenant}:{Task}] Starting per-tenant job...", tenantId, taskId);
        Thread.Sleep(3000);
        _logger.LogInformation("[{Tenant}:{Task}] Finished per-tenant job", tenantId, taskId);
    }
}
