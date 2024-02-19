using AutoMapper;
using MassTransit;
using MassTransitSagas.Contracts;
using MassTransitSagas.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitSagas.WebApi.Controllers;

[Route("api/test")]
[ApiController]
public class TestController(ILogger<TestController> _logger, IMapper _mapper) : ControllerBase
{


    [HttpPost("publish-test-message")]
    public async Task<IActionResult> PublishTestMessage(
        [FromBody] TestMessageRequest testMessageRequest,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken ct)
    {

        _logger.LogInformation("Sending TestMessage: {Text}", testMessageRequest.Text);

        await publishEndpoint.Publish(_mapper.Map<TestMessage>(testMessageRequest), ct);

        return Accepted();
    }
}
