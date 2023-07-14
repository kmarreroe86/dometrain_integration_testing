using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.Tests.Integration.CustomerController
{
    public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {

        private readonly CustomerApiFactory _apiFactory;

        public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
        }

        [Fact]
        public async Task Test()
        {
            await Task.Delay(3000);
        }
    }
}
