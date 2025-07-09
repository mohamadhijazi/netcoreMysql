using Microsoft.AspNetCore.Mvc;
using VoiceBot.Services;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LlmController : ControllerBase
{
    private readonly ILlmService _llmService;
    public LlmController(ILlmService llmService) => _llmService = llmService;

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] LlmRequest request, CancellationToken ct)
    {
        var response = await _llmService.GetResponseAsync(request.Prompt, request.SessionId, request.Model, request.Hardware, ct);
        return Ok(new { response });
    }
}

public class LlmRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Model { get; set; } = "default";
    public string Hardware { get; set; } = "CPU";
}
