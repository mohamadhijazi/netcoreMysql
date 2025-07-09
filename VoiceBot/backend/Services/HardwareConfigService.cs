using System.Threading.Tasks;
using VoiceBot.Data;
using Microsoft.EntityFrameworkCore;

namespace VoiceBot.Services
{
    public class HardwareConfigService : IHardwareConfigService
    {
        private readonly VoiceBotDbContext _db;
        public HardwareConfigService(VoiceBotDbContext db) => _db = db;

        public async Task<string> GetHardwareModeAsync(string userId)
        {
            if (!int.TryParse(userId, out var uid))
                return "cpu";
            var config = await _db.HardwareConfigs.FirstOrDefaultAsync(h => h.UserId == uid);
            return config?.Hardware ?? "cpu";
        }

        public async Task SetHardwareModeAsync(string userId, string mode)
        {
            if (!int.TryParse(userId, out var uid))
                return;
            var config = await _db.HardwareConfigs.FirstOrDefaultAsync(h => h.UserId == uid);
            if (config == null)
            {
                config = new Models.HardwareConfig { UserId = uid, Hardware = mode };
                _db.HardwareConfigs.Add(config);
            }
            else
            {
                config.Hardware = mode;
            }
            await _db.SaveChangesAsync();
        }
    }
}
