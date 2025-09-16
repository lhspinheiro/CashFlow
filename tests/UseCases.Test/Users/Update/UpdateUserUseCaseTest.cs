using CashFlow.Application.UseCases.Users.Update;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.User;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Users.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        var useCase = CreateUseCase(user);
        var act = async() => await useCase.Execute(request);
        await act.Should().NotThrowAsync();
        
        user.Name.Should().Be(request.Name);
        user.Email.Should().Be(request.Email);
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    [InlineData(null)]
    public async Task Error_Name_Empty(string name)
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = name;
        
        var useCase = CreateUseCase(user);
        var act = async() => await useCase.Execute(request);
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.NAME_EMPTY));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    [InlineData(null)]
    public async Task Error_Email_Empty(string email)
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = email;
        
        var useCase = CreateUseCase(user);
        var act = async() => await useCase.Execute(request);
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.EMAIL_EMPTY));
    }
    
    [Fact]
    public async Task Error_Email_Invalid()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "luis.com";
        
        var useCase = CreateUseCase(user);
        var act = async() => await useCase.Execute(request);
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.EMAIL_INVALID));
    }
    
    [Fact]
    public async Task Error_Email_Exist()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        
        var useCase = CreateUseCase(user, request.Email);
        var act = async() => await useCase.Execute(request);
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(e => e.GetErros().Count == 1 && e.GetErros().Contains(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
    }
    
    private UpdateUserUseCase CreateUseCase(User user, string? email = null)
    {
        var unitOfWOrk = UnitOfWorkBuilder.Build();
        var updateRepository = UserUpdateOnlyRepositoryBuilder.Build(user);
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepository = new UserReadOnlyRepositoryBuilder();

        if (string.IsNullOrWhiteSpace(email) == false)
        {
            readRepository.ExistActiveEmail(email);
        }
        
        return new UpdateUserUseCase(loggedUser, updateRepository, readRepository.Build(), unitOfWOrk);
    }
}