using Customers.Api;
using Customers.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("mydb")
            .WithUsername("postgres")
            .WithPassword("password")
            .Build();


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
                    new NpgsqlConnectionFactory("Server=localhost;Port=5555;Database=mydb;User ID=postgres;Password=password;"));
            });
        }


        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }


        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}
