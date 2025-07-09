using System.Threading.Tasks;

namespace VoiceBot.Services
{
    public interface IHardwareConfigService
    {
        Task<string> GetHardwareModeAsync(string userId);
        Task SetHardwareModeAsync(string userId, string mode);
    }
}
