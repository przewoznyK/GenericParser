using System.ComponentModel.DataAnnotations;

namespace GenericParser.Models
{
    public class ParseContentRequest
    {
        [Required]
        public ContentType Type { get; set; }
        [Required]
        public string Content { get; set; } = null!;
    }
}