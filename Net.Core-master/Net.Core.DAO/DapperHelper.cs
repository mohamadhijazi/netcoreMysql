namespace Net.Core.DAO
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class DapperHelper
    {
        public static IEnumerable<T> ExecuteSP<T>(DapperContext context, string name, DynamicParameters parm)
        {
            using (var connection = context.CreateConnection())
            {
                var results = connection.Query<T>(name, parm, null, true
                    , null, CommandType.StoredProcedure);
                return results;
            }

        }
        public static int ExecuteScalar(DapperContext context, string name, DynamicParameters parm)
        {
            using (var connection = context.CreateConnection())
            {
                var results = connection.ExecuteScalar<int>(name, parm, null
                    , null, CommandType.StoredProcedure);
                return results;
            }

        }
        public static async Task<IEnumerable<T>> ExecuteSPAsync<T>(DapperContext context, string name, DynamicParameters parm)
        {
            using (var connection = context.CreateConnection())
            {
                var results = await connection.QueryAsync<T>(name, parm, null
                    , null, CommandType.StoredProcedure);
                return results;
            }

        }
    }
}
