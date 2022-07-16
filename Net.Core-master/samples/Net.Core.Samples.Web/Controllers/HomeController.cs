using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using net.core.Samples.Web.Models.HomeViewModels;
using Net.Core.Common.Interfaces;

namespace Net.Core.Web.Controllers
{
    public class HomeController : Controller
    {
        public ICustomer CustomersServices { get; set; }
        public HomeController(ICustomer CustomersServices)
        {
            this.CustomersServices = CustomersServices;
        }
        public IActionResult Index()
        {
            return View(new IndexViewModel { Customers = this.CustomersServices.GetCustomers().Result });
        }

        public IActionResult Customer(int customerID)
        {
            return View(new CustomerViewModel { Customer = this.CustomersServices.GetCustomerInFormation(customerID) });
        }

        // GET: /Home/AddAccount
        public IActionResult AddAccount(int customerID)
        {
            if (customerID != 0)
            {
                return View(new AddAccountViewModel() { CustomerID = customerID });
            }
            return View();
        }

        // POST: /Home/AddAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAccount(AddAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accountId = this.CustomersServices.CreateAccount(model.CustomerID, model.InitialCredit);
            //if (InitialCredit != 0)
            //{
            //    var service = Application.ServiceProvider.GetService<ITransaction>();

            //    service.Create(accountId, model.InitialCredit);
            //}
            return RedirectToAction(nameof(Customer), new { customerID = model.CustomerID });
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
