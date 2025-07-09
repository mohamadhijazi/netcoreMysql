using System.Net.Http.Json;
namespace VoiceBot.Services;
public class PythonTtsService : ITtsService
{
    private readonly HttpClient _httpClient;
    public PythonTtsService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<byte[]> SynthesizeAsync(string text, string language, string voice, string emotion, string hardware, CancellationToken ct)
    {
        var payload = new { text, language, voice, emotion, hardware };
        var response = await _httpClient.PostAsJsonAsync("/tts/synthesize", payload, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(ct);
    }
}
