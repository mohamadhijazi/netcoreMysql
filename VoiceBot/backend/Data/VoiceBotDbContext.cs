using Microsoft.EntityFrameworkCore;
using VoiceBot.Models;

namespace VoiceBot.Data;

public class VoiceBotDbContext : DbContext
{
    public VoiceBotDbContext(DbContextOptions<VoiceBotDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<HardwareConfig> HardwareConfigs { get; set; }
    public DbSet<CsvRecord> CsvRecords { get; set; }
    public DbSet<Log> Logs { get; set; }
}
