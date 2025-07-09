using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using VoiceBot.Models;
using Microsoft.EntityFrameworkCore;
using VoiceBot.Data;

namespace VoiceBot.Services
{
    public class CsvService : ICsvService
    {
        private readonly VoiceBotDbContext _db;
        public CsvService(VoiceBotDbContext db) => _db = db;

        public async Task ImportCsvAsync(byte[] csvData)
        {
            var csvText = Encoding.UTF8.GetString(csvData);
            using var reader = new StringReader(csvText);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var fields = line.Split(',');
                // Expecting: FileName, Data
                if (fields.Length >= 2)
                {
                    var record = new CsvRecord { FileName = fields[0], Data = fields[1] };
                    _db.CsvRecords.Add(record);
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task<byte[]> ExportCsvAsync()
        {
            var records = await _db.CsvRecords.ToListAsync();
            var sb = new StringBuilder();
            foreach (var r in records)
            {
                sb.AppendLine($"{r.FileName},{r.Data}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<IEnumerable<string>> QueryCsvAsync(string query)
        {
            // Return all records where FileName or Data contains the query
            var results = await _db.CsvRecords
                .Where(r => r.FileName.Contains(query) || (r.Data ?? "").Contains(query))
                .Select(r => r.FileName + "," + r.Data)
                .ToListAsync();
            return results;
        }
    }
}
