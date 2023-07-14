using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Customers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;

namespace Customer.Api.Tests.Integration
{
    // WebApplicationFactory<IApiMarker> // Knows what Api initialize by the generic type, is a marker interface in Customer.Api assembly
    public class CustomerControllerTestsDemo2 : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
    {

        private readonly HttpClient _httpClient;

        private readonly Faker<CustomerRequest> _customerReqGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.GitHubUsername, "kmarreroe86")
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

        private readonly List<Guid> _createdIds = new();

        public CustomerControllerTestsDemo2(WebApplicationFactory<IApiMarker> appFactory)
        {
            _httpClient = appFactory.CreateClient(); // Create in-memory api that can be accessed with the HttpClient that returns.
        }


        [Fact]
        public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Act
            var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            // Dealing with it as text
            var text = await response.Content.ReadAsStringAsync();
            text.Should().Contain("404");

            // Dealing with Json
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem!.Title.Should().Be("Not Found");
            problem.Status.Should().Be(404);

            // Headers and more
            //response.Headers.Location!.ToString().Should().Be("");
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
