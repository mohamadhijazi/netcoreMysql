using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
namespace VoiceBot.Services;
public class PythonLlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    public PythonLlmService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<string> GetResponseAsync(string prompt, string sessionId, string model, string hardware, CancellationToken ct)
    {
        var payload = new { prompt, session_id = sessionId, model, hardware };
        var response = await _httpClient.PostAsJsonAsync("/llm", payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LlmResponse>(cancellationToken: ct);
        return result?.Response ?? string.Empty;
    }
    private class LlmResponse { public string Response { get; set; } = string.Empty; }
}
