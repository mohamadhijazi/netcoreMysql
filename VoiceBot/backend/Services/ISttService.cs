namespace VoiceBot.Services;
public interface ISttService
{
    Task<string> TranscribeAsync(byte[] audio, string language, string model, string hardware, CancellationToken ct);
}
