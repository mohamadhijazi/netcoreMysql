namespace Net.Core.Common.Interfaces
{
    using Net.Core.Common.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITransaction
    {
        public int Create(int accountID, float amount);

        public IList<Transaction> GetCustomerTransactions(int customerID);

    }
}
