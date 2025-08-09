using CashFlow.Application.UseCases.Expenses;
using CashFlow.Communication.Enums;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Expenses.Register;

public class RegisterExpenseValidatorTests
{

    [Fact]
    public void Sucess()
    {
        //Arrange - configurar as instâncias 

        var validator = new ExpenseValidator();
        var request = RequestRegisterExpenseBuilder.Build();

        //Act
        var result = validator.Validate(request);

        //Assert - Retonrar um resultado
        result.IsValid.Should().BeTrue();

    }

    [Theory]
    [InlineData("")]
    [InlineData("       ")]
    [InlineData(null)]
    public void Error_Title_empty(string title)
    {
        //Arrange - configurar as instâncias 
        var validator = new ExpenseValidator();
        var request = RequestRegisterExpenseBuilder.Build();
        request.Title = title;

        //Act
        var result = validator.Validate(request);

        //Assert - Retonrar um resultado
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.TITLE_REQUIRED));

    }


    [Fact]
    public void Error_Data_Future()
    {
        //Arrange - configurar as instâncias 
        var validator = new ExpenseValidator();
        var request = RequestRegisterExpenseBuilder.Build();
        request.Date = DateTime.UtcNow.AddDays(1);

        //Act
        var result = validator.Validate(request);

        //Assert - Retonrar um resultado
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EXPENSE_CANNOT_FUTURE));

    }

    [Fact]
    public void Error_Payment_Type_Invalid()
    {
        //Arrange - configurar as instâncias 
        var validator = new ExpenseValidator();
        var request = RequestRegisterExpenseBuilder.Build();
        request.paymentType = (PaymentType)700;

        //Act
        var result = validator.Validate(request);

        //Assert - Retonrar um resultado
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_INVALID));

    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Error_Amount_Invalid(decimal amount)
    {
        //Arrange - configurar as instâncias 
        var validator = new ExpenseValidator();
        var request = RequestRegisterExpenseBuilder.Build();
        request.Amount = amount;


        //Act
        var result = validator.Validate(request);

        //Assert - Retonrar um resultado
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_GREATER));

    }


}
