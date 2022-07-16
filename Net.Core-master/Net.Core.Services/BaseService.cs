namespace Net.Core.Services
{
    using Net.Core.DAO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class BaseService
    {
        public readonly DapperContext _context;

        public BaseService(DapperContext context)
        {
            _context = context;
        }
    }
}
