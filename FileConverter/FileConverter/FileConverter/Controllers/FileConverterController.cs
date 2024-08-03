
using DinkToPdf;
using DinkToPdf.Contracts;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FileConverterController : ControllerBase
  {
    private readonly ILogger<FileConverterController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConverter _converter;

    public FileConverterController(ILogger<FileConverterController> logger,
      IWebHostEnvironment webHostEnvironment,
      IConverter converter)
    {
      _logger = logger;
      _webHostEnvironment = webHostEnvironment;
      _converter = converter;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      string wwwrootPath = _webHostEnvironment.WebRootPath;
      string txtFilePath = Path.Combine(wwwrootPath, "Hello.txt");

      if (!System.IO.File.Exists(txtFilePath))
      {
        return NotFound("File not found");
      }

      string pdfFilePath = Path.Combine(wwwrootPath, "Hello.pdf");

      // Read the text file contents
      string textContent = await System.IO.File.ReadAllTextAsync(txtFilePath);

      // Convert the text file to PDF using iText7
      using (var writer = new PdfWriter(pdfFilePath))
      {
        using (var pdf = new PdfDocument(writer))
        {
          Document document = new Document(pdf);
          document.Add(new Paragraph(textContent));
        }
      }

      // Read the generated PDF into a MemoryStream
      var memory = new MemoryStream();
      using (var stream = new FileStream(pdfFilePath, FileMode.Open))
      {
        await stream.CopyToAsync(memory);
      }
      memory.Position = 0;

      // Optionally, delete the generated PDF file if you don't want to keep it on disk
      System.IO.File.Delete(pdfFilePath);

      // Return the PDF file
      return File(memory, "application/pdf", "Hello2.pdf");
    }


    [HttpGet]
    [Route("csv")]
    public async Task<IActionResult> GetCSV()
    {
      string wwwrootPath = _webHostEnvironment.WebRootPath;
      string csvFilePath = Path.Combine(wwwrootPath, "csvx.csv");

      if (!System.IO.File.Exists(csvFilePath))
      {
        return NotFound("CSV file not found");
      }

      // Read CSV file
      string csvContent = await System.IO.File.ReadAllTextAsync(csvFilePath);

      // Convert CSV content to HTML
      string htmlContent = ConvertCsvToHtml(csvContent);

      // Create PDF document
      var pdfDocument = new HtmlToPdfDocument
      {
        GlobalSettings = {
                DocumentTitle = "CSV to PDF",
                PaperSize = PaperKind.A4
            },
        Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent
                }
            }
      };

      // Convert HTML to PDF
      byte[] pdfBytes = _converter.Convert(pdfDocument);

      return File(pdfBytes, "application/pdf", "converted.pdf");

    }

    private string ConvertCsvToHtml(string csvContent)
    {
      var lines = csvContent.Split('\n');
      var html = "<html><body><table border='1'>";

      foreach (var line in lines)
      {
        var cells = line.Split(',');
        html += "<tr>";
        foreach (var cell in cells)
        {
          html += $"<td>{cell}</td>";
        }
        html += "</tr>";
      }

      html += "</table></body></html>";
      return html;
    }
  }



}
