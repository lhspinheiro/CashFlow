using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Users.Delete;

public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
{
  
    private readonly IUserWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteUserAccountUseCase( IUserWriteOnlyRepository repository,  
        IUnitOfWork unitOfWork, ILoggedUser loggedUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }
    
    public async Task Execute()
    {
        var loggedUser = await _loggedUser.Get();
        
        await _repository.Delete(loggedUser);
        await _unitOfWork.Commit();
    }
}