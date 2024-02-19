using AutoMapper;
using FluentValidation;
using MassTransit;
using MassTransitSagas.Contracts;
using MediatR;

namespace MassTransitSagas.WebApi.Requests;

// one-way request
public record TestMessageRequest : IRequest
{
    public string Text { get; init; } = string.Empty;
    public string? Tag { get; init; }
}

// validator (enabled on controller actions)
public class TestMessageRequestValidator : AbstractValidator<TestMessageRequest>
{
    public TestMessageRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MinimumLength(2);
    }
}

// automapper profile
public class TestMessageRequestMapping : Profile
{
    public TestMessageRequestMapping()
    {
        CreateMap<TestMessageRequest, TestMessage>();
    }
}

// one-way request handler
public class TestMessageRequestHandler : IRequestHandler<TestMessageRequest>
{
    private readonly ILogger<TestMessageRequestHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public TestMessageRequestHandler(
        ILogger<TestMessageRequestHandler> logger,
        IPublishEndpoint publishEndpoint,
        IMapper mapper)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }

    public async Task Handle(TestMessageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending TestMessage: {Text}", request.Text);
        await _publishEndpoint.Publish(_mapper.Map<TestMessage>(request), cancellationToken);
    }
}