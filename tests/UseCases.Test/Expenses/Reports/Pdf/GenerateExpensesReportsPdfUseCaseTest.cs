using CashFlow.Application.UseCases.Expenses.Reports.PDF;
using CashFlow.Domain.Entities;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Expenses.Reports.Pdf;

public class GenerateExpensesReportsPdfUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var loggedUser = UserBuilder.Build();
        var expenses = ExpenseBuilder.Collection(loggedUser);
        
        var useCase = CreateUseCase(loggedUser, expenses);
        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Sucess_Empety()
    {
        var loggedUser = UserBuilder.Build();
        var useCase = CreateUseCase(loggedUser, []);
        
        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));
        
        result.Should().BeEmpty();
    }

    private GenerateExpensesReportPdfUseCase CreateUseCase(User user, List<Expense> expenses)
    {
        var repository = new ExpenseReadOnlyRepositoryBuilder().FilterByMonth(user,expenses).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        
        return new GenerateExpensesReportPdfUseCase(repository, loggedUser);
    }
}