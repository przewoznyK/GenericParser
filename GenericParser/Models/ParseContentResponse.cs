namespace GenericParser.Models
{
    public class ParseContentResponse
    {
        public ParseStatus Status { get; set; }
        public int ProcessedCount { get; set; }
        public IReadOnlyCollection<Dictionary<string, object?>> Data { get; set; } = [];
    }
}