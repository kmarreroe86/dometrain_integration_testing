using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.Tests.Integration
{
    /// <summary>
    /// Share context amount test classes.
    /// See CreateCustomerController.cs, GetCustomerController.cs
    /// </summary>
    [CollectionDefinition("CustomerApi Collection")]
    public class TestCollection : IClassFixture<WebApplicationFactory<IApiMarker>>
    {

    }
}
