using System.Net.Http.Headers;
namespace VoiceBot.Services;
public class PythonSttService : ISttService
{
    private readonly HttpClient _httpClient;
    public PythonSttService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<string> TranscribeAsync(byte[] audio, string language, string model, string hardware, CancellationToken ct)
    {
        using var content = new MultipartFormDataContent();
        var audioContent = new ByteArrayContent(audio);
        audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
        content.Add(audioContent, "file", "audio.wav");
        content.Add(new StringContent(language), "language");
        content.Add(new StringContent(model), "model");
        content.Add(new StringContent(hardware), "hardware");
        var response = await _httpClient.PostAsync("/stt/transcribe", content, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SttResponse>(cancellationToken: ct);
        return result?.Text ?? string.Empty;
    }
    private class SttResponse { public string Text { get; set; } = string.Empty; }
}
