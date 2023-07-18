﻿using Customers.Api;
using Customers.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Customer.Api.Tests.Integration
{
    public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {

        public const string ValidGithubUser = "validuser";

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:alpine")
            .WithDatabase("mydb")
            .WithUsername("postgres")
            .WithPassword("password")
            .Build();

        private readonly GitHubApiServer _gitHubApiServer = new();


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbConnectionFactory));
                services.AddSingleton<IDbConnectionFactory>(_ =>
                    // new NpgsqlConnectionFactory("Server=localhost;Port=5555;Database=mydb;User ID=postgres;Password=password;"));
                    new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));

                services.AddHttpClient("GitHub", httpClient =>
                {
                    httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.Accept, "application/vnd.github.v3+json");
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
                });

            });
        }


        public async Task InitializeAsync()
        {
            _gitHubApiServer.Start();
            _gitHubApiServer.SetUpUser(ValidGithubUser);
            await _dbContainer.StartAsync();
        }


        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            _gitHubApiServer.Dispose();
        }
    }
}
