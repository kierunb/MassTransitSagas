using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    var entryAssembly = Assembly.GetEntryAssembly();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(entryAssembly);

    builder.Services.AddValidatorsFromAssembly(entryAssembly);
    builder.Services.AddFluentValidationAutoValidation(); // auto validation for controllers

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r =>
            r.AddService("MassTransit.WebAPI",
                serviceVersion: "1.0",
                serviceInstanceId: Environment.MachineName))
        .WithTracing(b => b
            .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
            .AddAspNetCoreInstrumentation()
            //.AddConsoleExporter()
            .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); }))   // for jeager with OLTP endpoint
         .WithMetrics(metrics => metrics
            .AddProcessInstrumentation()                // CPU, RAM, etc.
            .AddRuntimeInstrumentation()                // .NET runtime metrics - GC, ThreadPool, etc.
            .AddAspNetCoreInstrumentation()             // ASP.NET Core metrics
            .AddMeter(InstrumentationOptions.MeterName) // MassTransit meters
                                                        //.AddConsoleExporter()
            .AddPrometheusExporter());

    builder.Services.AddMassTransit(x =>
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

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseOpenTelemetryPrometheusScrapingEndpoint(); //    /metrics

    app.MapHealthChecks("/healthz");

    app.MapGet("/", () => "Hello in WebApi Client. Visit '/swagger' for list of endpints.");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


