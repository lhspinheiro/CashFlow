using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;
public interface IExpensesReadOnlyRepository
{
    public Task<List<Expense>> GetAll(Entities.User user);
    public Task<Expense?> GetById(Entities.User user, long id);

    public Task <List<Expense>> FilterByMonth(Entities.User user, DateOnly date);

}
