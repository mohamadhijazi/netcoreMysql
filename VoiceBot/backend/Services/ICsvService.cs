using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoiceBot.Services
{
    public interface ICsvService
    {
        Task ImportCsvAsync(byte[] csvData);
        Task<byte[]> ExportCsvAsync();
        Task<IEnumerable<string>> QueryCsvAsync(string query);
    }
}
