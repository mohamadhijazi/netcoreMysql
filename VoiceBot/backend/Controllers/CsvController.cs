using Microsoft.AspNetCore.Mvc;
using VoiceBot.Services;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CsvController : ControllerBase
{
    private readonly ICsvService _csvService;
    public CsvController(ICsvService csvService) => _csvService = csvService;

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromForm] IFormFile file, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        await _csvService.ImportCsvAsync(ms.ToArray());
        return Ok();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(CancellationToken ct)
    {
        var csv = await _csvService.ExportCsvAsync();
        return File(csv, "text/csv", "export.csv");
    }

    [HttpGet("query")]
    public async Task<IActionResult> Query([FromQuery] string q, CancellationToken ct)
    {
        var results = await _csvService.QueryCsvAsync(q);
        return Ok(results);
    }
}
