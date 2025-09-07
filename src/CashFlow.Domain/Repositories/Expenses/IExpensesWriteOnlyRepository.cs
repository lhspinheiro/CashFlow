using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;
public interface IExpensesWriteOnlyRepository
{
    public Task Add(Expense expense);
    
    public Task Delete(long id);    
}
