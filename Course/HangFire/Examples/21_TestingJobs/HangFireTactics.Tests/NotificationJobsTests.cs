using Hangfire;
using Hangfire.States;
using HangFireTactics.Web.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HangFireTactics.Tests;

public class NotificationJobsTests
{
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<ILogger<NotificationJobs>> _mockLogger;
    private readonly NotificationJobs _sut;

    public NotificationJobsTests()
    {
        _mockEmailSender = new Mock<IEmailSender>();
        _mockLogger = new Mock<ILogger<NotificationJobs>>();
        _sut = new NotificationJobs(_mockEmailSender.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SendWelcomeEmail_CallsEmailSender_WithCorrectParameters()
    {
        var userId = "user123";
        var email = "test@example.com";

        await _sut.SendWelcomeEmail(userId, email);

        _mockEmailSender.Verify(x => x.SendAsync(
            email,
            "Welcome!",
            It.Is<string>(body => body.Contains(userId))),
            Times.Once);
    }

    [Fact]
    public async Task SendOrderConfirmation_CallsEmailSender_WithOrderId()
    {
        var orderId = "ORD-12345";
        var email = "customer@example.com";

        await _sut.SendOrderConfirmation(orderId, email);

        _mockEmailSender.Verify(x => x.SendAsync(
            email,
            "Order Confirmed",
            It.Is<string>(body => body.Contains(orderId))),
            Times.Once);
    }
}

public class BackgroundJobClientTests
{
    [Fact]
    public void Enqueue_CanBeMocked_ToVerifyJobEnqueued()
    {
        var mockClient = new Mock<IBackgroundJobClient>();
        mockClient
            .Setup(x => x.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<IState>()))
            .Returns("job-123");

        var controller = new FakeController(mockClient.Object);
        
        controller.EnqueueWelcomeEmail("user1", "test@test.com");

        mockClient.Verify(x => x.Create(
            It.Is<Hangfire.Common.Job>(job => 
                job.Type == typeof(NotificationJobs) &&
                job.Method.Name == nameof(NotificationJobs.SendWelcomeEmail)),
            It.IsAny<IState>()),
            Times.Once);
    }
}

public class FakeController
{
    private readonly IBackgroundJobClient _client;

    public FakeController(IBackgroundJobClient client)
    {
        _client = client;
    }

    public void EnqueueWelcomeEmail(string userId, string email)
    {
        _client.Enqueue<NotificationJobs>(j => j.SendWelcomeEmail(userId, email));
    }
}
