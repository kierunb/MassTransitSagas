using AutoMapper;
using FluentValidation;
using MassTransitSagas.Contracts;

namespace MassTransitSagas.WebApi.Requests;

public record TestMessageRequest
{
    public string Text { get; init; } = string.Empty;
    public string? Tag { get; init; }
}

public class TestMessageRequestValidator : AbstractValidator<TestMessageRequest>
{
    public TestMessageRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MinimumLength(2);
    }
}

public class TestMessageRequestMapping : Profile
{
    public TestMessageRequestMapping()
    {
        CreateMap<TestMessageRequest, TestMessage>();
    }
}