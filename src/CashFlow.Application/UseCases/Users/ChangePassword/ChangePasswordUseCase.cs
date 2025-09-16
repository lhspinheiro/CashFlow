using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Secutiry.Cryptography;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.ChangePassword;

public class ChangePasswordUseCase :  IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IpasswordEncripter  _passwordEncripter;

    public ChangePasswordUseCase(ILoggedUser loggedUser, IUserUpdateOnlyRepository repository, IpasswordEncripter  passwordEncripter,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _passwordEncripter = passwordEncripter;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(RequestChangePasswordJSon request)
    {
        var loggedUser = await _loggedUser.Get();
        
         await Validate(request, loggedUser);
        
        var user = await _repository.GetById(loggedUser.Id);
        user.Password = _passwordEncripter.Encrypt(request.NewPassword);
        
        _repository.Update(user);
        await _unitOfWork.Commit();

    }

    private async Task Validate(RequestChangePasswordJSon request, User loggedUser)
    {
        var validate = new ChangePasswordValidator();
        var result = await validate.ValidateAsync(request);
        
        var passwordMatch = _passwordEncripter.verify(request.Password, loggedUser.Password);

        if (passwordMatch == false)
        {
            result.Errors.Add(new ValidationFailure(String.Empty, ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
        }

        if (result.IsValid == false)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors); 
        }
        
    }
}