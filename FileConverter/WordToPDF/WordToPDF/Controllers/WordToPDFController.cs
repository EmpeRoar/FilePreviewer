using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DinkToPdf;
using DinkToPdf.Contracts;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
  private readonly IConverter _converter;

  public PdfController(IConverter converter)
  {
    _converter = converter;
  }

  [HttpGet("{fileName}")]
  public IActionResult ConvertToPdf(string fileName)
  {
    try
    {
      // Get the path to the Word document
      var wordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

      if (!System.IO.File.Exists(wordFilePath))
      {
        return NotFound("The specified file does not exist.");
      }

      // Convert Word to HTML
      string htmlContent = ConvertWordToHtml(wordFilePath);

      // Convert HTML to PDF using DinkToPdf
      var pdfDoc = new HtmlToPdfDocument()
      {
        GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                },
        Objects = {
                    new ObjectSettings {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
      };

      var pdfBytes = _converter.Convert(pdfDoc);

      // Return the PDF as a file
      var outputFileName = Path.GetFileNameWithoutExtension(fileName) + ".pdf";
      return File(pdfBytes, "application/pdf", outputFileName);
    }
    catch (Exception ex)
    {
      // Handle exceptions appropriately
      // Logging the exception can be added here if needed
      return BadRequest(ex.Message);
    }
  }

  private string ConvertWordToHtml(string wordFilePath)
  {
    StringBuilder htmlContent = new StringBuilder();

    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(wordFilePath, false))
    {
      MainDocumentPart mainPart = wordDoc.MainDocumentPart;

      htmlContent.Append("<html><body>");

      foreach (var element in mainPart.Document.Body.Elements())
      {
        if (element is Paragraph)
        {
          htmlContent.Append("<p>");
          foreach (var run in element.Elements<Run>())
          {
            foreach (var text in run.Elements<Text>())
            {
              htmlContent.Append(text.Text);
            }
          }
          htmlContent.Append("</p>");
        }
        else if (element is Table)
        {
          htmlContent.Append("<table border='1'>");
          foreach (var row in element.Elements<TableRow>())
          {
            htmlContent.Append("<tr>");
            foreach (var cell in row.Elements<TableCell>())
            {
              htmlContent.Append("<td>");
              foreach (var cellContent in cell.Elements<Paragraph>())
              {
                foreach (var run in cellContent.Elements<Run>())
                {
                  foreach (var text in run.Elements<Text>())
                  {
                    htmlContent.Append(text.Text);
                  }
                }
              }
              htmlContent.Append("</td>");
            }
            htmlContent.Append("</tr>");
          }
          htmlContent.Append("</table>");
        }
      }

      htmlContent.Append("</body></html>");
    }

    return htmlContent.ToString();
  }
}
