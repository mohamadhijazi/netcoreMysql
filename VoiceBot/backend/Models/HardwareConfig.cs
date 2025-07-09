namespace VoiceBot.Models;
public class HardwareConfig
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Module { get; set; } = string.Empty; // TTS, STT, LLM
    public string Hardware { get; set; } = string.Empty; // CPU, GPU
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
