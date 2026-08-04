using GenericParser.Exceptions;
using GenericParser.Interfaces;
using GenericParser.Models;
using System.Text.Json;

namespace GenericParser.Parsers
{
    public class InternalJsonParser : IContentParser
    {
        public ContentType SupportedType => ContentType.INTERNAL_JSON;

        public Task<IReadOnlyCollection<Dictionary<string, object?>>> ParseAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidContentException("JSON content cannot be empty");
            }

            var result = new List<Dictionary<string, object?>>();

            try
            {
                using var document = JsonDocument.Parse(content);

                if (document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidContentException("Internal JSON must be an array");
                }

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    if (element.ValueKind != JsonValueKind.Object)
                    {
                        throw new InvalidContentException("Array must contain JSON objects.");
                    }

                    var item = new Dictionary<string, object?>();

                    foreach (var property in element.EnumerateObject())
                    {
                        item[property.Name] = ConvertJsonValue(property.Value);
                    }

                    result.Add(item);
                }
            }
            catch (JsonException)
            {
                throw new InvalidContentException("Provided content is not valid JSON");
            }

            return Task.FromResult<IReadOnlyCollection<Dictionary<string, object?>>>(result);
        }

        private static object? ConvertJsonValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.ToString()
            };
        }
    }
}