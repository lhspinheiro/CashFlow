using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Login
{
    public interface IDoLoginUSeCase
    {
         public Task<ResponseRegisteredUserJson> Execute(RequestLoginJSon request);
    }
}