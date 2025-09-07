namespace CashFlow.Domain.Secutiry.Tokens;

public interface ITokenProvider
{
    string TokenOnRequest();
}