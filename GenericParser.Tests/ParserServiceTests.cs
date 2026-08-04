using FluentAssertions;
using GenericParser.Exceptions;
using GenericParser.Interfaces;
using GenericParser.Models;
using GenericParser.Parsers;
using GenericParser.Services;
using System.Text;

namespace GenericParser.Tests
{
    public class ParserServiceTests
    {
        private readonly ParserService _service;

        public ParserServiceTests()
        {
            var parsers = new List<IContentParser>
            {
                new CsvParser(),
                new InternalJsonParser()
            };

            _service = new ParserService(parsers);
        }

        [Fact]
        public async Task ParseAsync_ShouldDecodeBase64AndParseCsv()
        {
            // Arrange
            var csv = """
                      name,age
                      John,30
                      Anna,25
                      """;

            // Act
            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(csv))
            };

            var result = await _service.ParseAsync(request);

            // Assert
            result.Status.Should().Be(ParseStatus.Success);
            result.ProcessedCount.Should().Be(2);
            result.Data.First()["name"].Should().Be("John");
            result.Data.First()["age"].Should().Be("30");
        }

        [Fact]
        public async Task ParseAsync_ShouldDecodeBase64AndParseInternalJson()
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
            var result = await _service.ParseAsync(request);

            // Assert
            result.Status.Should().Be(ParseStatus.Success);
            result.ProcessedCount.Should().Be(2);
            result.Data.First()["name"].Should().Be("John");
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenBase64IsInvalid()
        {
            // Arrange
            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = "not-valid-base64"
            };

            // Act
            Func<Task> action = async () => await _service.ParseAsync(request);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenContentIsEmpty()
        {
            // Arrange
            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = ""
            };

            // Act
            Func<Task> action = async () => await _service.ParseAsync(request);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenParserIsNotRegistered()
        {
            // Arrange
            var parsers = new List<IContentParser>();
            var service = new ParserService(parsers);

            var csv = """
                      name,age
                      John,30
                      Anna,25
                      """;

            // Act
            var request = new ParseContentRequest
            {
                Type = ContentType.CSV,
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(csv))
            };

            Func<Task> action = async () => await service.ParseAsync(request);

            // Assert
            await action.Should().ThrowAsync<UnsupportedContentTypeException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenRequestIsNull()
        {
            // Act
            Func<Task> action = async () => await _service.ParseAsync(null!);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>().WithMessage("Request cannot be null");
        }
    }
}