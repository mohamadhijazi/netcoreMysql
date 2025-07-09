using Microsoft.AspNetCore.Mvc;
using VoiceBot.Services;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HardwareConfigController : ControllerBase
{
    private readonly IHardwareConfigService _hardwareConfigService;
    public HardwareConfigController(IHardwareConfigService hardwareConfigService) => _hardwareConfigService = hardwareConfigService;

    [HttpGet("mode")]
    public async Task<IActionResult> GetMode([FromQuery] string userId, CancellationToken ct)
    {
        var mode = await _hardwareConfigService.GetHardwareModeAsync(userId);
        return Ok(new { mode });
    }

    [HttpPost("mode")]
    public async Task<IActionResult> SetMode([FromBody] SetModeRequest request, CancellationToken ct)
    {
        await _hardwareConfigService.SetHardwareModeAsync(request.UserId, request.Mode);
        return Ok();
    }
}

public class SetModeRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Mode { get; set; } = "cpu";
}
