using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using FluentAssertions;

namespace UseCases.Test.Users.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        
        var useCase = CreateUseCase();
        var act = async () => await useCase.Execute(request);
        
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(ex => ex.GetErros().Count == 1 && ex.GetErros().Contains(ResourceErrorMessages.NAME_EMPTY));
    }

    [Fact]
    public async Task Error_Email_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        
        request.Email = string.Empty;
        
        var useCase = CreateUseCase();
        var act = async () => await useCase.Execute(request);
        
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(ex => ex.GetErros().Count == 1 && ex.GetErros().Contains(ResourceErrorMessages.EMAIL_EMPTY));
        
    }

    [Fact]
    public async Task Error_Password_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = string.Empty;
        var useCase = CreateUseCase();
        var act = async () => await useCase.Execute(request);
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
       result.Where(ex => ex.GetErros().Count == 1 && ex.GetErros().Contains(ResourceErrorMessages.INVALID_PASSWORD));
       
    }

    [Fact]
    public async Task Error_Email_Exist()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
         var useCase = CreateUseCase(request.Email);
        
        var act = async () => await useCase.Execute(request); 
        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
        result.Where(ex => ex.GetErros().Count == 1 && ex.GetErros().Contains(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
    }

    private RegisterUserUseCase CreateUseCase(string? email = null)
    { 
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var writeOnlyRepository = UserWriteOnlyRepositoryBuilder.Builder();
        var passwordEncripter = new PasswordEncrypterBuilder().Build();
        var jwtTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder();

        if (string.IsNullOrEmpty(email) == false)
        {
            readRepository.ExistActiveEmail(email);
        }
        
        return new RegisterUserUseCase(mapper, passwordEncripter, readRepository.Build(), writeOnlyRepository, unitOfWork, jwtTokenGenerator); 
    }
}