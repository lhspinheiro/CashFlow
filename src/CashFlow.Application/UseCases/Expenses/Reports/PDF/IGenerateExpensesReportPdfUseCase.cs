namespace CashFlow.Application.UseCases.Expenses.Reports.PDF;
public interface IGenerateExpensesReportPdfUseCase
{

    public Task<byte[]> Execute(DateOnly month);


}
