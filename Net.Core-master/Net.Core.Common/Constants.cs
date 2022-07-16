using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Core.Common
{
    public static class Constants
    {
        public struct SP
        {
            public const string GetCustomers = "sp_GetCustomers";
            public const string GetCustomerById = "sp_GetCustomerByID";
            public const string GetCustomerBalanceByID = "sp_GetCustomerBalanceByID";
            public const string GetCustomerTransactionsByID = "sp_GetCustomerTransactionsByID";
            public const string CreateTransaction = "sp_Transaction";
            public const string CreateAccount = "sp_CreateAccount";


        }
    }
}
