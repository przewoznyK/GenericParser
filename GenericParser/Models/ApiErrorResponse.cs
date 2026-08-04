namespace GenericParser.Models
{
    public class ApiErrorResponse
    {
        public ParseStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}