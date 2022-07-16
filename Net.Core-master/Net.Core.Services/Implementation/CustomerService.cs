
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

    public class CustomerService : BaseService, ICustomer
    {
        public CustomerService(DapperContext context) : base(context)
        {
        }


        public Customer GetCustomerInFormation(int customerID)
        {
            var parms = new DynamicParameters();
            parms.Add("parm_ID", customerID);
            var customer = DapperHelper.ExecuteSP<Customer>(_context, Constants.SP.GetCustomerBalanceByID, parms).FirstOrDefault();
            customer.Transactions = DapperHelper.ExecuteSP<Transaction>(_context, Constants.SP.GetCustomerTransactionsByID, parms);
            return customer;
        }
        public int CreateAccount(int customerID, float InitialCredit)
        {
            var parms = new DynamicParameters();
            parms.Add("parm_ID", customerID);
            var id = DapperHelper.ExecuteScalar(_context, Constants.SP.CreateAccount, parms);

            //if (InitialCredit != 0)
            //{
            //    parms.Add("parm_Amount", InitialCredit);
            //    DapperHelper.ExecuteScalar(_context, Constants.SP.CreateTransaction, parms);
            //}
            if (InitialCredit != 0)
            {
                var service = Application.ServiceProvider.GetService<ITransaction>();

                service.Create(id, InitialCredit);
            }

            return id;
        }
        public Task<IEnumerable<Customer>> GetCustomers()
        {
            return DapperHelper.ExecuteSPAsync<Customer>(_context, Constants.SP.GetCustomers, null);
        }
    }
}
