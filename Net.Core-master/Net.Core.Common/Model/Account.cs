namespace Net.Core.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Account
    {
        public int ID { get; set; }

        public int CustomerID { get; set; }

        public long Balance { get; set; }
    }
}
