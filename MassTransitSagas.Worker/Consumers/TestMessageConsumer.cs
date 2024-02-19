using MassTransit;
using MassTransitSagas.Contracts;

namespace MassTransitSagas.Worker.Consumers;

public class TestMessageConsumer(ILogger<TestMessageConsumer> _logger) : IConsumer<TestMessage>
{
    public async Task Consume(ConsumeContext<TestMessage> context)
    {
        _logger.LogInformation("Received TestMessage: {Text}", context.Message.Text);
        await Task.CompletedTask;
    }
}
