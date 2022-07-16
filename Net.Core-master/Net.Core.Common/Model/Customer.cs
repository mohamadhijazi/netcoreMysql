namespace Net.Core.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float Balance { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }

    }
}
