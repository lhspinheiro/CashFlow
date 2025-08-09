using CashFlow.Application.UseCases.Login;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using FluentAssertions;

namespace UseCases.Test.Login;

public class DoLoginUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var userBuilder = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = userBuilder.Email;
        var useCase = CreateUseCase(userBuilder, request.Password);
        
        var result = await useCase.Execute(request);
        
        result.Should().NotBeNull();
        result.Name.Should().Be(userBuilder.Name);
        result.Token.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Error_User_Not_Found()
    {
        var userBuilder = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();

        var useCase = CreateUseCase(userBuilder, request.Password);
        
        var act = async () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<InvalidLoginException>();
        result.Where(ex =>
            ex.GetErros().Count == 1 && ex.GetErros().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));
    }
    
    [Fact]
    public async Task Error_Password_Invalid()
    {
        var userBuilder = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = userBuilder.Email;
        
        var useCase = CreateUseCase(userBuilder);
        
        var act = async () => await useCase.Execute(request);
        
        var result = await act.Should().ThrowAsync<InvalidLoginException>();
        result.Where(errorPassword => errorPassword.GetErros().Count == 1 && errorPassword.GetErros().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));
    }
    private DoLoginUSeCase CreateUseCase(CashFlow.Domain.Entities.User user, string? password = null)
    {
        var passwordEncrypter = new PasswordEncrypterBuilder().Verify(password).Build();
        
        var jwtTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder().GetUserByEmail(user).Build();
        
        return new DoLoginUSeCase(readRepository,  passwordEncrypter, jwtTokenGenerator);
    }
}