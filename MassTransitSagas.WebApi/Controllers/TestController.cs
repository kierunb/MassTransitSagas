using AutoMapper;
using MassTransit;
using MassTransitSagas.Contracts;
using MassTransitSagas.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitSagas.WebApi.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{

    // verion without MediatR
    [HttpPost("publish-test-message")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PublishTestMessage(
        [FromBody] TestMessageRequest testMessageRequest,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] IMapper _mapper,
        [FromServices] ILogger<TestController> _logger,
        CancellationToken ct)
    {
        _logger.LogInformation("Sending TestMessage: {Text}", testMessageRequest.Text);

        await publishEndpoint.Publish(_mapper.Map<TestMessage>(testMessageRequest), ct);

        return Accepted();
    }

    [HttpPost("publish-test-message-mediatr")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PublishTestMessageMediatR(
        [FromBody] TestMessageRequest testMessageRequest,
        [FromServices] IMediator _mediatr,
        CancellationToken ct)
    {
        await _mediatr.Send(testMessageRequest, ct);
        return Accepted();
    }
}
