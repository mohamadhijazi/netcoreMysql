
namespace Net.Core.Services.Implementation
{
    using Net.Core.Common.Interfaces;
    using Net.Core.Common.Model;
    using Net.Core.DAO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Net.Core.Common;
    using Dapper;
    using Microsoft.Extensions.DependencyInjection;

    public class TransactionService : BaseService, ITransaction
    {
        public TransactionService(DapperContext context) : base(context)
        {
        }

        public int Create(int accountID, float amount)
        {
            var parms = new DynamicParameters();
            parms.Add("parm_ID", accountID);
            parms.Add("parm_Amount", amount);

            return DapperHelper.ExecuteScalar(_context, Constants.SP.CreateTransaction, parms);

        }

        public IList<Transaction> GetCustomerTransactions(int customerID)
        {
            throw new NotImplementedException();
        }
    }
}
