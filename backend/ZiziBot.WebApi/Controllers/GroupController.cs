using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ApiControllerBase
{
    [HttpGet("welcome-message")]
    public async Task<IActionResult> GetWelcomeMessage(GetWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("welcome-message")]
    public async Task<IActionResult> SaveWelcomeMessage(SaveWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("select-welcome-message")]
    public async Task<IActionResult> SelectWelcomeMessage(SelectWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }
}