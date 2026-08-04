using GenericParser.Exceptions;
using GenericParser.Interfaces;
using GenericParser.Models;

namespace GenericParser.Parsers
{
    public class CsvParser : IContentParser
    {
        public ContentType SupportedType => ContentType.CSV;

        public Task<IReadOnlyCollection<Dictionary<string, object?>>> ParseAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidContentException("CSV content cannot be empty.");
            }

            if (content.TrimStart().StartsWith("[") ||
                content.TrimStart().StartsWith("{"))
            {
                throw new InvalidContentException("Provided content is not CSV.");
            }

            var result = new List<Dictionary<string, object?>>();

            var lines = content.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            var headers = lines[0]
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();

            if (headers.Length < 2)
            {
                throw new InvalidContentException("Invalid CSV format.");
            }

            foreach (var line in lines.Skip(1))
            {
                var values = line
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToArray();

                if (values.Length != headers.Length)
                {
                    throw new InvalidContentException("CSV row has invalid number of columns.");
                }

                var row = new Dictionary<string, object?>();

                for (int i = 0; i < headers.Length; i++)
                {
                    row[headers[i]] = values[i];
                }

                result.Add(row);
            }

            return Task.FromResult<IReadOnlyCollection<Dictionary<string, object?>>>(result);
        }
    }
}