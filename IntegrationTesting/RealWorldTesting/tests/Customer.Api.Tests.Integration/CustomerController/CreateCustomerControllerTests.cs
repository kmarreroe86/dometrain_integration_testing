﻿using Bogus;
using Customers.Api;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using SharpYaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.Tests.Integration.CustomerController
{
    public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {

        // private readonly CustomerApiFactory _apiFactory;
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUser)
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);


        public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
        {
            // _apiFactory = apiFactory;
            _client = apiFactory.CreateClient();
        }


        [Fact]
        public async Task Create_CreatesUser_WhenDataIsValid()
        {

            // Arange
            var customer = _customerGenerator.Generate();

            // Act
            var response = await _client.PostAsJsonAsync("customers", customer);

            // Assert
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location!.ToString().Should().Be($"http://localhost/customers/{customerResponse.Id}");

        }


        [Fact]
        public async Task Create_ReturnsValidationError_WhenEmailIsInvalid()
        {
            // Arrange
            const string invalidEmail = "invalid-email";
            var customer = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail).Generate();


            // Act
            var response = await _client.PostAsJsonAsync("customers", customer);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
            error.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
        }


        [Fact]
        public async Task Create_ReturnsValidationError_WhenGithubUserDoesNotExists()
        {
            // Arrange
            const string invalidUser = "invalid-user";
            var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidUser).Generate();


            // Act
            var response = await _client.PostAsJsonAsync("customers", customer);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
            error.Errors["GitHubUsername"][0].Should().Be($"There is no GitHub user with username {invalidUser}");
        }
    }
}
