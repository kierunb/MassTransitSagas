using MassTransit;
using MassTransit.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

ConfigureOpenTelemetry();

try
{
    var entryAssembly = Assembly.GetEntryAssembly();

    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.SetInMemorySagaRepositoryProvider();

                x.AddConsumers(entryAssembly);
                x.AddSagaStateMachines(entryAssembly);
                x.AddSagas(entryAssembly);
                x.AddActivities(entryAssembly);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            // Workers
            //services.AddHostedService<Worker>();
        })
        .UseSerilog()
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static void ConfigureOpenTelemetry()
{
    // OpenTelemetry configuration
    Sdk.CreateTracerProviderBuilder()
        .ConfigureResource(r =>
            r.AddService("MassTransit.Worker",
                serviceVersion: "1.0",
                serviceInstanceId: Environment.MachineName))
        //.AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
        .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
                                                          //.AddConsoleExporter() // Any OTEL suportable exporter can be used here
        .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); })   // for jeager with OLTP endpoint
        .Build();
}



