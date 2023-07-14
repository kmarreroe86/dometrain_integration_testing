using Bogus;
using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;

namespace Customer.Api.Tests.Integration.CustomerController
{
    [Collection("CustomerApi Collection")]
    public class CreateCustomerController : IAsyncLifetime
    {

        private readonly HttpClient _httpClient;

        private readonly Faker<CustomerRequest> _customerReqGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.GitHubUsername, "kmarreroe86")
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

        private readonly List<Guid> _createdIds = new();

        public CreateCustomerController(WebApplicationFactory<IApiMarker> appFactory)
        {
            _httpClient = appFactory.CreateClient(); // Create in-memory api that can be accessed with the HttpClient that returns.
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCustomerIsCreated()
        {
            var customer = _customerReqGenerator.Generate();

            var response = await _httpClient.PostAsJsonAsync("customers", customer);

            // Assert
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            _createdIds.Add(customerResponse!.Id);
        }

        public Task InitializeAsync() => Task.CompletedTask;


        public async Task DisposeAsync()
        {
            foreach (var id in _createdIds)
            {
                await _httpClient.DeleteAsync($"customers/{id}");
            }

        }
    }
}
