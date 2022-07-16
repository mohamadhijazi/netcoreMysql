using Microsoft.AspNetCore.Mvc;
using Net.Core.Common.Interfaces;
using Net.Core.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace net.core.Samples.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        public ICustomer CustomersServices { get; set; }

        public Account(ICustomer CustomersServices)
        {
            this.CustomersServices = CustomersServices;
        }
        // GET: api/<Account>
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
           return await this.CustomersServices.GetCustomers();
           // return new string[] { "value1", "value2" };
        }

        // GET api/<Account>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Account>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Account>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Account>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
