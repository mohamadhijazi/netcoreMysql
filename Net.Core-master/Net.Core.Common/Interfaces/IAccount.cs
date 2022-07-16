namespace Net.Core.Common.Interfaces
{
    using Net.Core.Common.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAccount
    {
        public int Create(int customerID, long initialCredit);

    }
}
