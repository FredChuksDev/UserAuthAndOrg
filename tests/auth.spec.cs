using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace UserAuthAndOrg.tests
{
    public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_Successfully()
        {
            var user = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                password = "Password123!",
                phone = "1234567890"
            };
            var response = await _client.PostAsync("/auth/register", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal("success", (string)responseData.status);
            Assert.Equal("Registration successful", (string)responseData.message);
        }

        [Fact]
        public async Task LoginUser_Successfully()
        {
            var login = new
            {
                email = "john.doe@example.com",
                password = "Password123!"
            };
            var response = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal("success", (string)responseData.status);
            Assert.Equal("Login successful", (string)responseData.message);
        }

        [Fact]
        public async Task RegisterUser_ValidationErrors()
        {
            var user = new
            {
                firstName = "",
                lastName = "",
                email = "invalidemail",
                password = "",
                phone = "1234567890"
            };
            var response = await _client.PostAsync("/auth/register", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal("Bad request", (string)responseData.status);
        }

        [Fact]
        public async Task RegisterUser_DuplicateEmail()
        {
            var user = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                password = "Password123!",
                phone = "1234567890"
            };
            await _client.PostAsync("/auth/register", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            var response = await _client.PostAsync("/auth/register", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal("Bad request", (string)responseData.status);
        }
    }
}
