
namespace Net.Core.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Transaction
    {
        public int ID { get; set; }

        public int AccountID { get; set; }
        public long Amount { get; set; }

    }
}
