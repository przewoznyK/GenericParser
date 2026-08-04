using GenericParser.Models;

namespace GenericParser.Interfaces
{
    public interface IContentParser
    {
        ContentType SupportedType { get; }
        Task<IReadOnlyCollection<Dictionary<string, object?>>> ParseAsync(string content);
    }
}