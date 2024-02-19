namespace MassTransitSagas.Contracts;

public record TestMessage
{
    public string Text { get; init; } = string.Empty;
    public string? Tag { get; init; }
}
