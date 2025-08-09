using CashFlow.Application.UseCases.Expenses.Reports.Excel;
using CashFlow.Application.UseCases.Expenses.Reports.PDF;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CashFlow.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{

   // Endpoint para gerar o excel

    [HttpGet("excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    public async Task<IActionResult> GetExcel([FromServices] IGenerateExpensesReportExcelUseCase useCase , [FromHeader] DateOnly month)
    {
        byte[] file = await useCase.Execute(month);


        if (file.Length > 0) //caso exista dados dentro da varíavel file que representa o arquivo, vai devolver o arquivo.
            return File(file, MediaTypeNames.Application.Octet, "report.xlsx");

       return NoContent();
    }


    // Endpoint para gerar o PDF

    [HttpGet("pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    public async Task<IActionResult> GetPdf([FromServices] IGenerateExpensesReportPdfUseCase useCase, [FromQuery] DateOnly month)
    {
        byte[] file = await useCase.Execute(month);

        if (file.Length > 0) 
            return File(file, MediaTypeNames.Application.Pdf, "report.pdf");

        return NoContent();
      
    }

}
