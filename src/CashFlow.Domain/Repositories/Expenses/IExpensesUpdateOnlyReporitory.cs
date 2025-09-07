using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;
public interface IExpensesUpdateOnlyReporitory
{

    Task<Expense?> GetById(Entities.User user,long id);
    public void Update(Expense expense);
}
