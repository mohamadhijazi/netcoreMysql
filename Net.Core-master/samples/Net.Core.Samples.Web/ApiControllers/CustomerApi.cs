using Microsoft.AspNetCore.Mvc;
using net.core.Samples.Web.Models.HomeViewModels;
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
    public class CustomerApi : ControllerBase
    {
        public ICustomer CustomersServices { get; set; }

        public CustomerApi(ICustomer CustomersServices)
        {
            this.CustomersServices = CustomersServices;
        }
        // GET: api/<Customer>
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await this.CustomersServices.GetCustomers();
            // return new string[] { "value1", "value2" };
        }

        // GET api/<Customer>/5
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return this.CustomersServices.GetCustomerInFormation(id);

        }

        // POST api/<Customer>
        [HttpPost]
        public void Post([FromBody] AddAccountViewModel model)
        {
            var accountId = this.CustomersServices.CreateAccount(model.CustomerID, model.InitialCredit);
        }

        // PUT api/<Customer>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Customer>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
