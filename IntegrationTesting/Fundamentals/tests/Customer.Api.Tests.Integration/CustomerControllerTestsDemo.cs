using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.Tests.Integration
{
    public class CustomerControllerTestsDemo
    {

        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001")
        };

        //[Fact]
        [Theory]
        //[InlineData("4c8d790f-f5df-4a95-94a6-c2069e6f180c")]
        //[InlineData("4c8d790f-f5df-4a95-94a6-c2069e6f1800")]

        //[MemberData(nameof(Data))]

        [ClassData(typeof(ClassData))]
        public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist(string uuidStr)
        {

            // Arrange
            //var httpClient = new HttpClient
            //{
            //    BaseAddress = new Uri("https://localhost:5001")
            //};

            // Act
            var response = await _httpClient.GetAsync($"customers/{Guid.Parse(uuidStr)}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("9FD9A907-E86E-43BC-9A80-F2B0F274133F", Skip = "This doesn't work atm sorry")]
        [InlineData("45905930-B282-4330-891F-381BD951D2BC")]
        [InlineData("6728cd57-1e42-4fd0-9867-75a090fa2ce3")]
        public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist2(
       string guidAsText)
        {
            // Act
            var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public static IEnumerable<object[]> Data { get; } = new[]
        {
            new []{ "4c8d790f-f5df-4a95-94a6-c2069e6f180c" },
            new []{ "4c8d790f-f5df-4a95-94a6-c2069e6f1800" }
        };

    }

    public class ClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "4c8d790f-f5df-4a95-94a6-c2069e6f180c" };
            yield return new object[] { "4c8d790f-f5df-4a95-94a6-c2069e6f1800" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
