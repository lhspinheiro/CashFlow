using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.DataAccess.Repositories;
internal class ExpensesRepository : IExpensesReadOnlyRepository, IExpensesWriteOnlyRepository, IExpensesUpdateOnlyReporitory
{

    private readonly CashFlowDbContext _dbContext;
    public ExpensesRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Expense expense)
    {

        await _dbContext.Expenses.AddAsync(expense);

    }

    public async Task<bool> Delete(long id)
    {

        var result = await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
        if (result is null)
        {
            return false;
        }
         _dbContext.Expenses.Remove(result);

        return true;
        
    }

    public async Task<List<Expense>> GetAll()
    {
        return await _dbContext.Expenses.AsNoTracking().ToListAsync();
    }

     async Task<Expense?> IExpensesReadOnlyRepository.GetById(long id)
    {
        return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(expense => expense.Id == id);
    }

     async Task<Expense?> IExpensesUpdateOnlyReporitory.GetById(long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public async Task<List<Expense>> FilterByMonth(DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day:1).Date; //00h do dia passado

        var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month); //quantidade de dias que tem no mes passado

        var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth, hour: 23, minute: 59, second:59); 


       return await _dbContext.Expenses.AsNoTracking()
            .Where(expense => expense.Date >= startDate && expense.Date <= endDate) //vai pegar o mês passado e devolver todas as despesas do mês
            .OrderBy(expense => expense.Date)  //vai ordenar as despesas de acordo com as datas
            .ThenBy(expense => expense.Title) //caso tenha 2 despesas na mesma data e no mesmo horário, vai ordenar por titulo
            .ToListAsync();
    }
}
