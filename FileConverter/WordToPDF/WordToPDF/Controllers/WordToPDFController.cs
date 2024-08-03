

using Microsoft.AspNetCore.Mvc;


namespace WordToPDF.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WordToPDFController : ControllerBase
  {
    private readonly ILogger<WordToPDFController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public WordToPDFController(ILogger<WordToPDFController> logger,
      IWebHostEnvironment webHostEnvironment)
    {
      _logger = logger;
      _webHostEnvironment = webHostEnvironment;

    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      string wwwrootPath = _webHostEnvironment.WebRootPath;
      string wordFilePath = Path.Combine(wwwrootPath, "word.docx");

      return Ok();

    }
  }
}
