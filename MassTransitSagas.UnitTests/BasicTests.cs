using MassTransit;
using MassTransit.Testing;
using MassTransitSagas.Worker.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MassTransitSagas.UnitTests;

public class BasicTests
{
    [Fact]
    public async Task TestHarness_Works()
    {
        await using var provider = new ServiceCollection()
        //.AddYourBusinessServices() // register all of your normal business services
        .AddSingleton<ILoggerFactory, NullLoggerFactory>()
        .AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<TestMessageConsumer>();
        })
        .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();

        }
        finally
        {
            await harness.Stop();
        }

        Assert.True(true);
    }
}