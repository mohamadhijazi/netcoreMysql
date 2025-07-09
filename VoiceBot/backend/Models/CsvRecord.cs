namespace VoiceBot.Models;
public class CsvRecord
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? Data { get; set; }
    public int? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
