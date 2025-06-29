// BaseService provides a base class for service classes, encapsulating common functionality.
namespace Net.Core.Services
{
    using Net.Core.DAO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    /// <summary>
    /// Base class for all service classes, providing access to the DapperContext for database operations.
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// The DapperContext instance used for database access.
        /// </summary>
        public readonly DapperContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="context">The DapperContext to be used for database operations.</param>
        public BaseService(DapperContext context)
        {
            _context = context;
        }
    }
}
// End of BaseService.cs
