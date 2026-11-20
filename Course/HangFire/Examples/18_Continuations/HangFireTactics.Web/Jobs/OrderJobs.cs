namespace HangFireTactics.Web.Jobs;

public class OrderJobs
{
    private readonly ILogger<OrderJobs> _logger;

    public OrderJobs(ILogger<OrderJobs> logger)
    {
        _logger = logger;
    }

    public void ValidateOrder(string orderId)
    {
        _logger.LogInformation("[{OrderId}] Step 1: Validating order...", orderId);
        Thread.Sleep(2000);
        _logger.LogInformation("[{OrderId}] Order validated!", orderId);
    }

    public void ProcessPayment(string orderId)
    {
        _logger.LogInformation("[{OrderId}] Step 2: Processing payment...", orderId);
        Thread.Sleep(2000);
        _logger.LogInformation("[{OrderId}] Payment processed!", orderId);
    }

    public void ShipOrder(string orderId)
    {
        _logger.LogInformation("[{OrderId}] Step 3: Shipping order...", orderId);
        Thread.Sleep(2000);
        _logger.LogInformation("[{OrderId}] Order shipped!", orderId);
    }

    public void SendConfirmation(string orderId)
    {
        _logger.LogInformation("[{OrderId}] Step 4: Sending confirmation email...", orderId);
        Thread.Sleep(1000);
        _logger.LogInformation("[{OrderId}] Confirmation sent!", orderId);
    }
}
