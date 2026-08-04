using GenericParser.Models;

namespace GenericParser.Interfaces
{
    public interface IParserService
    {
        Task<ParseContentResponse> ParseAsync(ParseContentRequest request);
    }
}