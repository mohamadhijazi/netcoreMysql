using Net.Core.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.core.Samples.Web.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
    }
}
