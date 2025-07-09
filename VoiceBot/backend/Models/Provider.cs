namespace VoiceBot.Models;
public class Provider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // TTS, STT, LLM
    public byte[]? ApiKey { get; set; }
    public string? Endpoint { get; set; }
    public string? Settings { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
