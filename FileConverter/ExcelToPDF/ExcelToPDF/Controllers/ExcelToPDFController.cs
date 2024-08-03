using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ExcelToPDF.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ExcelToPDFController : ControllerBase
  {
    private readonly ILogger<ExcelToPDFController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    

    public ExcelToPDFController(ILogger<ExcelToPDFController> logger,
      IWebHostEnvironment webHostEnvironment)
    {
      _logger = logger;
      _webHostEnvironment = webHostEnvironment;
    
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      string wwwrootPath = _webHostEnvironment.WebRootPath;
      string excelFilePath = Path.Combine(wwwrootPath, "excelx.xlsx");

      if (!System.IO.File.Exists(excelFilePath))
      {
        return NotFound("Excel file not found");
      }

      using (var workbook = new XLWorkbook(excelFilePath))
      {
        var worksheet = workbook.Worksheets.Worksheet(1);

        using (var pdfStream = new MemoryStream())
        {
          var document = new Document();
          PdfWriter.GetInstance(document, pdfStream);

          document.Open();

          // Create a simple PDF content from Excel sheet (customize as needed)
          foreach (var row in worksheet.Rows())
          {
            var paragraph = new Paragraph();
            foreach (var cell in row.Cells())
            {
              paragraph.Add(cell.Value.ToString() + " "); // Add space or any separator you want
            }
            document.Add(paragraph);
          }

          document.Close();

          // Reset the stream position to the beginning before returning
          // pdfStream.Position = 0;

          // Return the PDF as a FileResult
          return File(pdfStream.ToArray(), "application/pdf", "converted.pdf");
        }
      }
    }
  }
}
