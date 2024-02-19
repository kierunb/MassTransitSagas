# MassTransit with Sagas App Starter

- Projects are preconfigured to use MassTransit with RabbitMQ and SQL Server (with Entity Framework).
- WebApi is used to send commands and publish events and use MassTransit, AutoMapper and FluentValidation.
- Serilog, OpenTelemetry, Seq and Jaeger are used for logging and tracing.
    - Optionally Seq may also be used for tracing 
- Prometheus and Grafana may be used for metrics collection and visualization.

## Required Infrastructure

### RabbitMQ

```bash
$ docker run --name rabbitmq -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

## Optional and recommended infrastructure

### Seq

```bash
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -v seqvolume:/data -p 5342:80 -p 5341:5341 datalust/seq
```

### Jaeger

```bash
docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 -p 5775:5775/udp -p 6831:6831/udp -p 6832:6832/udp -p 5778:5778 -p 16686:16686 -p 14268:14268 -p 9411:9411 jaegertracing/all-in-one:1.6
```

#### Other

- SQL Server - required for Saga persistance
- Prometheus - required for metrics collecton (App, MassTransit, RabbitMQ)
- Grafana - required for metrics visualzation
    - ASP.NET Core dashboards: https://grafana.com/orgs/dotnetteam
    - RabbitMQ dashboards: https://grafana.com/grafana/dashboards/10991-rabbitmq-overview/
    - MassTransit dashboards: https://grafana.com/grafana/dashboards/17680-masstransit-messages-monitoring/