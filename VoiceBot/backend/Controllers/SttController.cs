using Microsoft.AspNetCore.Mvc;
using VoiceBot.Services;

namespace VoiceBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SttController : ControllerBase
{
    private readonly ISttService _sttService;
    public SttController(ISttService sttService)
    {
        _sttService = sttService;
    }

    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe([FromForm] SttRequest request, IFormFile file, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var text = await _sttService.TranscribeAsync(ms.ToArray(), request.Language, request.Model, request.Hardware, ct);
        return Ok(new { text });
    }
}

public class SttRequest
{
    public string Language { get; set; } = "en";
    public string Model { get; set; } = "base";
    public string Hardware { get; set; } = "CPU";
}
