namespace VoiceBot.Models;
public class Session
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? Context { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
