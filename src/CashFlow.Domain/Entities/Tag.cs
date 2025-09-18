namespace CashFlow.Domain.Entities;

public class Tag
{
    public long id {get; set;}
    public Enums.Tag ValueTag {get; set;}
    public long ExpenseId {get; set;}
    public Expense Expense {get; set;} = default!;
}