namespace Net.Core.Common.Interfaces
{
    using Net.Core.Common.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICustomer
    {
        public Customer GetCustomerInFormation(int customerID);

        public Task<IEnumerable<Customer>> GetCustomers();

        public int CreateAccount(int customerID, float InitialCredit);

    }
}
