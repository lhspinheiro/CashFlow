using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.Users.ChangePassword;

public interface IChangePasswordUseCase
{
    public Task Execute(RequestChangePasswordJSon request); 
}