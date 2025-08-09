using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Secutiry.Tokens;
public interface IAccessTokenGenerator
{
    string Generate(User user);

}
