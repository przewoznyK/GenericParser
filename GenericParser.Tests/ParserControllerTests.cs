using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using GenericParser.Models;
using System.Net.Http.Json;
using FluentAssertions;
using System.Net;

namespace GenericParser.Tests
{
    public class ParserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ParserControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ParseContent_ShouldReturnParsedCsv()
        {
            // Arrange
            var csv = """
                      name,age
                      John,30
                      Anna,25
                      """;

            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(csv))
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content",request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("\"processedCount\":2");
            body.Should().Contain("John");
            body.Should().Contain("Anna");
        }

        [Fact]
        public async Task ParseContent_ShouldReturnParsedInternalJson()
        {
            // Arrange
            var json = """
                       [
                        {
                          "id": 1,
                          "name": "John"
                        },
                        {
                          "id": 2,
                          "name": "Anna"
                        }
                       ]
                       """;

            var request = new ParseContentRequest
            {
                Type = ContentType.INTERNAL_JSON,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("\"processedCount\":2");
            body.Should().Contain("John");
            body.Should().Contain("Anna");
        }

        [Fact]
        public async Task ParseContent_ShouldReturnBadRequest_WhenContentIsInvalidBase64()
        {
            // Arrange
            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = "invalid-base64"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("not valid Base64");
        }

        [Fact]
        public async Task ParseContent_ShouldReturnBadRequest_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "abc";

            var request = new ParseContentRequest
            {
                Type = ContentType.INTERNAL_JSON,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidJson))
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("not valid JSON");
        }

        [Fact]
        public async Task ParseContent_ShouldReturnBadRequest_WhenRequestBodyIsEmpty()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content", (ParseContentRequest?)null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("A non-empty request body is required");
        }

        [Fact]
        public async Task ParseContent_ShouldReturnBadRequest_WhenCsvColumnsMismatch()
        {
            // Arrange
            var csv = """
                      name,age
                      John
                      """;

            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(csv))
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/parse-content", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("invalid number of columns");
        }
    }
}