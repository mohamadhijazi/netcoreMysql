using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Net.Core.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace net.core.Samples.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicEntityApiController : ControllerBase
    {
        private readonly DapperContext _context;
        private static readonly MemoryCache _schemaCache = new MemoryCache(new MemoryCacheOptions());
        private static readonly TimeSpan _schemaCacheDuration = TimeSpan.FromMinutes(10);

        public DynamicEntityApiController(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Accepts a JSON object and entity name, maps fields, and inserts into DB via stored procedure.
        /// </summary>
        /// <param name="entityName">The name of the entity/table.</param>
        /// <param name="inputJson">The input JSON object.</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string entityName, [FromBody] JsonElement inputJson)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                return BadRequest("Entity name is required.");

            // 1. Retrieve entity fields from DB (assuming information_schema.columns)
            var fields = await _context.QueryAsync<string>($@"
                SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @EntityName",
                new { EntityName = entityName });

            if (fields == null)
                return NotFound($"Entity '{entityName}' not found.");

            // 2. Map input JSON to entity fields and create parameters
            var parameters = new List<SqlParameter>();
            foreach (var field in fields)
            {
                if (inputJson.TryGetProperty(field, out var value))
                {
                    parameters.Add(new SqlParameter($"@{field}", value.ToString() ?? (object)DBNull.Value));
                }
            }

            // 3. Call stored procedure (assume naming convention: sp_Insert_{EntityName})
            var spName = $"sp_Insert_{entityName}";
            try
            {
                await _context.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return Ok("Insert successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Accepts an OData-like object and entity name, builds a SQL query, and returns filtered data as JSON.
        /// </summary>
        /// <param name="entityName">The name of the entity/table.</param>
        /// <param name="odata">The OData-like filter/select/limit object.</param>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string entityName, [FromBody] JsonElement odata)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                return BadRequest("Entity name is required.");
            if (odata.ValueKind != JsonValueKind.Object)
                return BadRequest("OData object is required in the request body.");

            // Get allowed fields from cache or DB
            var allowedFields = await GetAllowedFieldsAsync(entityName);
            if (allowedFields.Count == 0)
                return NotFound($"Entity '{entityName}' not found.");

            // Parse select
            string select = "*";
            if (odata.TryGetProperty("select", out var selectProp) && selectProp.ValueKind == JsonValueKind.Array)
            {
                var selectList = new List<string>();
                foreach (var item in selectProp.EnumerateArray())
                {
                    var field = item.GetString();
                    if (!string.IsNullOrWhiteSpace(field) && allowedFields.Contains(field))
                        selectList.Add(field);
                }
                if (selectList.Count > 0)
                    select = string.Join(", ", selectList);
            }

            // Parse filter (only allow equality on whitelisted fields)
            var whereClauses = new List<string>();
            var parameters = new List<SqlParameter>();
            if (odata.TryGetProperty("filter", out var filterProp) && filterProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in filterProp.EnumerateObject())
                {
                    var field = prop.Name;
                    if (allowedFields.Contains(field))
                    {
                        var paramName = "@p_" + field;
                        whereClauses.Add($"[{field}] = {paramName}");
                        parameters.Add(new SqlParameter(paramName, prop.Value.ToString() ?? (object)DBNull.Value));
                    }
                }
            }
            int? limit = odata.TryGetProperty("limit", out var limitProp) && limitProp.ValueKind == JsonValueKind.Number
                ? limitProp.GetInt32()
                : null;

            // Build SQL query
            var sql = $"SELECT {select} FROM [{entityName}]";
            if (whereClauses.Count > 0)
                sql += " WHERE " + string.Join(" AND ", whereClauses);
            if (limit.HasValue)
                sql += $" LIMIT {limit.Value}";

            try
            {
                using (var conn = _context.CreateConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);
                    var sqlCmd = cmd as SqlCommand;
                    if (sqlCmd == null)
                        throw new InvalidOperationException("Command is not a SqlCommand.");
                    using (var reader = await sqlCmd.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            result.Add(row);
                        }
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private async Task<List<string>> GetAllowedFieldsAsync(string entityName)
        {
            if (_schemaCache.TryGetValue(entityName, out List<string> allowedFields))
                return allowedFields;

            allowedFields = new List<string>();
            using (var conn = _context.CreateConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @EntityName";
                var param = cmd.CreateParameter();
                param.ParameterName = "@EntityName";
                param.Value = entityName;
                cmd.Parameters.Add(param);
                using (var reader = await (cmd as SqlCommand).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        allowedFields.Add(reader.GetString(0));
                }
            }
            _schemaCache.Set(entityName, allowedFields, _schemaCacheDuration);
            return allowedFields;
        }
    }
}
