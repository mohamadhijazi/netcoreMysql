namespace VoiceBot.Services;
public interface ILlmService
{
    Task<string> GetResponseAsync(string prompt, string sessionId, string model, string hardware, CancellationToken ct);
}
