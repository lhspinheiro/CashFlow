using CashFlow.Application.UseCases.Users.ChangePassword;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Users.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJSonBuilder.Build();
        var useCase = CreateUseCase(user, request.Password);
        var act = async () => useCase.Execute(request);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJSonBuilder.Build();
        request.NewPassword = string.Empty;
        
        var useCase = CreateUseCase(user, request.Password);
        var act = async () =>  await useCase.Execute(request);
        
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.INVALID_PASSWORD));
    }
    
    [Fact]
    public async Task Error_CurrentPassword_Different()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJSonBuilder.Build();
        
        var useCase = CreateUseCase(user);
        var act = async () =>  await useCase.Execute(request);
        
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
    }


    private ChangePasswordUseCase CreateUseCase(User user, string? password = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var updateRepository = UserUpdateOnlyRepositoryBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var passwordEncripter = new PasswordEncrypterBuilder().Verify(password).Build();
        
        return new ChangePasswordUseCase(loggedUser, updateRepository,passwordEncripter, unitOfWork);

    }
}