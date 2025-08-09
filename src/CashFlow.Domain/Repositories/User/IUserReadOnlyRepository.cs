namespace CashFlow.Domain.Repositories.User;
public interface IUserReadOnlyRepository
{
    Task <bool> ExistActiveUserWithEmail (string email);
    Task <Entities.User?> GetUserBYEmail(string email); // caso não encontre nenhuma email, o '?' indica que retornara nullo
}
