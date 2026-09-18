using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit;

namespace API.Tests.Integration
{
    public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test",
                            options => { });
                });
            });
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(
                "/api/v1/products?pageNumber=1&pageSize=10");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_InvalidPageNumber_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(
                "/api/v1/products?pageNumber=0&pageSize=10");

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_InvalidPageSize_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(
                "/api/v1/products?pageNumber=1&pageSize=101");

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(
                claims,
                "Test");

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(
                principal,
                "Test");

            return Task.FromResult(
                AuthenticateResult.Success(ticket));
        }
    }
}