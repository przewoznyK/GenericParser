using GenericParser.Exceptions;
using GenericParser.Interfaces;
using GenericParser.Models;
using System.Text;

namespace GenericParser.Services
{
    public class ParserService : IParserService
    {
        private readonly IEnumerable<IContentParser> _parsers;

        public ParserService(IEnumerable<IContentParser> parsers)
        {
            _parsers = parsers;
        }

        public async Task<ParseContentResponse> ParseAsync(ParseContentRequest request)
        {
            if (request == null)
            {
                throw new InvalidContentException("Request cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new InvalidContentException("Content cannot be empty");
            }

            var parser = _parsers.FirstOrDefault(x => x.SupportedType == request.Type);

            if (parser is null)
            {
                throw new UnsupportedContentTypeException($"Content type {request.Type} is not supported");
            }

            string decodedContent;

            try
            {
                var bytes = Convert.FromBase64String(request.Content);
                decodedContent = Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                throw new InvalidContentException("Content is not valid Base64");
            }

            var parsedData = await parser.ParseAsync(decodedContent);

            return new ParseContentResponse
            {
                Status = ParseStatus.Success,
                ProcessedCount = parsedData.Count,
                Data = parsedData
            };
        }
    }
}