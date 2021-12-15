using Common.Models.Requests.Customer;
using Common.Models.Responses.Customer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using UI.Interfaces.Services;
using UI.Models;
using UI.Models.ViewModels.Customers;

namespace UI.Controllers
{
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private IHttpClientService HttpClientService { get; set; }
        private JsonSerializerOptions apiJsonSerializerOptions;

        public CustomersController(IHttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            CustomerListViewModel customerListViewModel = new CustomerListViewModel
            {
                Customers = new List<CustomerViewModel>(),
            };

            APICallResult<CustomerListResponse> apiCallResult =
                await this.HttpClientService.MakeRequest<CustomerListResponse>(HttpMethod.Get, "http://localhost:40781/api/customers/")
                .ConfigureAwait(true);

            if (apiCallResult.IsSuccessStatusCode)
            {
                if (apiCallResult.ResultObject.Customers != null && apiCallResult.ResultObject.Customers.Count > 0)
                {
                    var customerResponses = apiCallResult.ResultObject.Customers;
                    customerResponses = customerResponses.Where(c=>c.IsActive).OrderBy(c => c.Name).ToList();
                    foreach (CustomerResponse cr in customerResponses)
                    {
                        customerListViewModel.Customers.Add(
                            new CustomerViewModel
                            {
                                CustomerId = cr.CustomerId,
                                Name = cr.Name,
                                CompanyRegistrationNumber = cr.CompanyRegistrationNumber,
                                IsActive = cr.IsActive,
                            });
                    }
                }
            }
            else
            {
                // Display error.
            }

            return View(customerListViewModel);
        }

        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> ProcessAdd(CustomerViewModel customerViewModel)
        {
            AddCustomerRequest addCustomerRequest = new AddCustomerRequest
            {
                Name = customerViewModel.Name,
                CompanyRegistrationNumber = customerViewModel.CompanyRegistrationNumber,
                IsActive = customerViewModel.IsActive,
            };

            APICallResult<CustomerResponse> apiCallResult = await this.HttpClientService.MakeRequest<CustomerResponse>(
                HttpMethod.Post,
                "http://localhost:40781/api/customers/",
                this.Serialize<AddCustomerRequest>(addCustomerRequest)).ConfigureAwait(true);

            if (apiCallResult.IsSuccessStatusCode)
            {
                // Update successful.
                return RedirectToAction("Index", "Customers");

            }
            else
            {
                return View("~/Views/Customers/Add.cshtml");
                // Display error.
            }

            
        }

        [HttpGet("Display/{customerId}")]
        public async Task<IActionResult> DisplayCustomer(int customerId)
        {
            CustomerViewModel cusViewModel = new CustomerViewModel();
            APICallResult<CustomerResponse> apiCallResult =
               await this.HttpClientService.MakeRequest<CustomerResponse>(HttpMethod.Get, "http://localhost:40781/api/customers/getCustomer/" + customerId)
               .ConfigureAwait(true);

            if (apiCallResult.IsSuccessStatusCode)
            {
                var customerResponse = apiCallResult.ResultObject;
                cusViewModel.CustomerId = customerResponse.CustomerId;
                cusViewModel.Name = customerResponse.Name;
                cusViewModel.CompanyRegistrationNumber = customerResponse.CompanyRegistrationNumber;
                cusViewModel.IncorporationDate = customerResponse.IncorporationDate;
                cusViewModel.IsActive = customerResponse.IsActive;
                cusViewModel.TurnOver = customerResponse.TurnOver;
               
            }
            else
            {

            }
            return View("Display", cusViewModel);
        }
        private string Serialize<T>(T objectToBeSerialized)
        {
            return JsonSerializer.Serialize<T>(objectToBeSerialized, this.ApiJsonSerializerOptions);
        }

        protected JsonSerializerOptions ApiJsonSerializerOptions
        {
            get
            {
                if (this.apiJsonSerializerOptions == null)
                {
                    this.apiJsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true,
                    };
                }

                return this.apiJsonSerializerOptions;
            }
        }
    }
}