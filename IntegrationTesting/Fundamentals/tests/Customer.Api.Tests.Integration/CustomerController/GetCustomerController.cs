using Customers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;

namespace Customer.Api.Tests.Integration.CustomerController
{
    [Collection("CustomerApi Collection")]
    public class GetCustomerController
    {
        private readonly HttpClient _httpClient;

        public GetCustomerController(WebApplicationFactory<IApiMarker> appFactory)
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
    }
}
