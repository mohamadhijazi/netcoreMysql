using Microsoft.AspNetCore.Mvc;
using VoiceBot.Services;

namespace VoiceBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TtsController : ControllerBase
{
    private readonly ITtsService _ttsService;
    public TtsController(ITtsService ttsService)
    {
        _ttsService = ttsService;
    }

    [HttpPost("synthesize")]
    public async Task<IActionResult> Synthesize([FromBody] TtsRequest request, CancellationToken ct)
    {
        var audio = await _ttsService.SynthesizeAsync(request.Text, request.Language, request.Voice, request.Emotion, request.Hardware, ct);
        return File(audio, "audio/wav");
    }
}

public class TtsRequest
{
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Voice { get; set; } = "default";
    public string Emotion { get; set; } = "neutral";
    public string Hardware { get; set; } = "CPU";
}
