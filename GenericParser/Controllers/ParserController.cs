using GenericParser.Exceptions;
using GenericParser.Interfaces;
using GenericParser.Models;
using Microsoft.AspNetCore.Mvc;

namespace GenericParser.Controllers
{
    [ApiController]
    [Route("api/v1/parse-content")]
    public class ParserController : ControllerBase
    {
        private readonly IParserService _parserService;

        public ParserController(IParserService parserService)
        {
            _parserService = parserService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ParseContentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ParseAsync([FromBody] ParseContentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = ParseStatus.Error,
                    Message = "Request cannot be null"
                });
            }

            try
            {
                var response = await _parserService.ParseAsync(request);
                return Ok(response);
            }
            catch (Exception ex) when (ex is InvalidContentException || ex is UnsupportedContentTypeException)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = ParseStatus.Error,
                    Message = ex.Message
                });
            }
        }
    }
}