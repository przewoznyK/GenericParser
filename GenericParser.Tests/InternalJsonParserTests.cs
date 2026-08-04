using FluentAssertions;
using GenericParser.Exceptions;
using GenericParser.Parsers;

namespace GenericParser.Tests
{
    public class InternalJsonParserTests
    {
        [Fact]
        public async Task ParseAsync_ShouldParseValidJson()
        {
            // Arrange
            var parser = new InternalJsonParser();

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

            // Act
            var result = await parser.ParseAsync(json);

            // Assert
            result.Should().HaveCount(2);
            result.First()["name"].Should().Be("John");
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenJsonInvalid()
        {
            // Arrange
            var parser = new InternalJsonParser();

            var json = """
                       abc
                       """;

            // Act
            Func<Task> action = async () => await parser.ParseAsync(json);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenJsonIsObject()
        {
            // Arrange
            var parser = new InternalJsonParser();

            var json = """
                       {
                         "name": "John"
                       }
                       """;

            // Act
            Func<Task> action = async () => await parser.ParseAsync(json);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenArrayContainsNonObject()
        {
            // Arrange
            var parser = new InternalJsonParser();

            var json = """
                       [
                         {
                           "id":1
                         },
                         "abc"
                       ]
                       """;

            // Act
            Func<Task> action = async () => await parser.ParseAsync(json);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenJsonIsEmpty()
        {
            // Arrange
            var parser = new InternalJsonParser();

            // Act
            Func<Task> action = async () => await parser.ParseAsync("");

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }
    }
}