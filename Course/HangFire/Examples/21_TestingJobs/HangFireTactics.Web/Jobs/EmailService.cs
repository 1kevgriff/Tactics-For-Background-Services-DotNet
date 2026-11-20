namespace HangFireTactics.Web.Jobs;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}

public class NotificationJobs
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationJobs> _logger;

    public NotificationJobs(IEmailSender emailSender, ILogger<NotificationJobs> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendWelcomeEmail(string userId, string email)
    {
        _logger.LogInformation("Preparing welcome email for user {UserId}", userId);
        await _emailSender.SendAsync(email, "Welcome!", $"Hello user {userId}!");
    }

    public async Task SendOrderConfirmation(string orderId, string email)
    {
        _logger.LogInformation("Preparing order confirmation for {OrderId}", orderId);
        await _emailSender.SendAsync(email, "Order Confirmed", $"Order {orderId} confirmed!");
    }
}
