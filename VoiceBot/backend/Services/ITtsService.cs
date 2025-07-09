namespace VoiceBot.Services;
public interface ITtsService
{
    Task<byte[]> SynthesizeAsync(string text, string language, string voice, string emotion, string hardware, CancellationToken ct);
}
