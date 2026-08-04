using FluentAssertions;
using GenericParser.Exceptions;
using GenericParser.Parsers;

namespace GenericParser.Tests
{
    public class CsvParserTests
    {
        [Fact]
        public async Task ParseAsync_ShouldParseValidCsv()
        {
            // Arrange
            var parser = new CsvParser();

            var csv = """
                      name,age
                      John,30
                      Anna,25
                      """;

            // Act 
            var result = await parser.ParseAsync(csv);

            // Assert
            result.Should().HaveCount(2);

            result.First()["name"].Should().Be("John");
            result.First()["age"].Should().Be("30");
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenColumnsMismatch()
        {
            // Arrange
            var parser = new CsvParser();

            var csv = """
                      name,age
                      John
                      """;

            // Act 
            Func<Task> action = async () => await parser.ParseAsync(csv);

            // Assert
            await action.Should().ThrowAsync<InvalidContentException>();
        }

        [Fact]
        public async Task ParseAsync_ShouldTrimSpaces()
        {
            // Arrange
            var parser = new CsvParser();

            var csv = """
                      name,age
                      John ,30
                      Anna, 25
                      """;
            // Act
            var result = await parser.ParseAsync(csv);

            // Assert
            result.First()["name"].Should().Be("John");
            result.First()["age"].Should().Be("30");
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenCsvIsEmpty()
        {
            // Arrange
            var parser = new CsvParser();

            // Act
            Func<Task> action = async () => await parser.ParseAsync("");

            // Assert
            await action.Should()
                .ThrowAsync<InvalidContentException>()
                .WithMessage("CSV content cannot be empty.");
        }

        [Fact]
        public async Task ParseAsync_ShouldThrow_WhenCsvHasOnlyOneColumn()
        {
            // Arrange
            var parser = new CsvParser();

            var csv = """
                      name
                      John
                      """;

            // Act
            Func<Task> action = async () => await parser.ParseAsync(csv);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidContentException>()
                .WithMessage("Invalid CSV format.");
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnEmpty_WhenCsvContainsOnlyHeaders()
        {
            // Arrange
            var parser = new CsvParser();

            var csv = """
                      name,age
                      """;

            // Act
            var result = await parser.ParseAsync(csv);

            // Assert
            result.Should().BeEmpty();
        }
    }
}