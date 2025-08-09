
using CashFlow.Domain.Enums;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;

public class GenerateExpensesReportExcelUseCase : IGenerateExpensesReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "€"; //o valor não pode ser alterado
    private readonly IExpensesReadOnlyRepository _repository;

    public GenerateExpensesReportExcelUseCase(IExpensesReadOnlyRepository reporitory)
    {
        _repository = reporitory;
    }


    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _repository.FilterByMonth(month);
        if (expenses.Count == 0)
        {
            return [];
        }

        using var workBook = new XLWorkbook(); //
        
        workBook.Author = "Luis Henrique";
        workBook.Style.Font.FontSize = 12;
        workBook.Style.Font.FontName = "Times New Roman";

        var workSheet = workBook.Worksheets.Add(month.ToString("Y")); // vai criar uma aba com a data passada

        InsertHeader(workSheet);


        var raw = 2;
        foreach (var expense in expenses) // vai adicionar as despesas solicitadas.
        {
            workSheet.Cell($"A{raw}").Value = expense.Title;
            workSheet.Cell($"B{raw}").Value = expense.Date;
            workSheet.Cell($"C{raw}").Value = expense.paymentType.PaymentTypeToString();               
            workSheet.Cell($"D{raw}").Value = expense.Amount;
            workSheet.Cell($"D{raw}").Style.NumberFormat.Format = $"-{CURRENCY_SYMBOL} #, ##0.00"; //formata como número com 2 casas decimais

            
            workSheet.Cell($"E{raw}").Value = expense.Description;
            raw++;
        }

        workSheet.Columns().AdjustToContents(); //vai ajustar as colunas -  ISSUES

        var file = new MemoryStream(); // utilizara a memoria em vez de um local especifico 
        workBook.SaveAs(file);

        return file.ToArray();

    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = ResourcerReportGenerationMessages.TITLE;
        worksheet.Cell("B1").Value = ResourcerReportGenerationMessages.DATE;
        worksheet.Cell("C1").Value = ResourcerReportGenerationMessages.PAYMENT_TYPE;
        worksheet.Cell("D1").Value = ResourcerReportGenerationMessages.AMOUNT;
        worksheet.Cell("E1").Value = ResourcerReportGenerationMessages.DESCRIPTION;
        worksheet.Cells("A1:E1").Style.Font.Bold = true;
        worksheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#FA8072");
        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        worksheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

    }
}
