using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _repository;

    public UserReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserReadOnlyRepository>();
    }

    public void ExistActiveEmail(string email)
    {
        _repository.Setup(user => user.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
    }

    public UserReadOnlyRepositoryBuilder GetUserByEmail(User user)
    {
         _repository.Setup(userRepository => userRepository.GetUserBYEmail(user.Email)).ReturnsAsync(user);
         
         return this;
    }
    public IUserReadOnlyRepository Build()=> _repository.Object;
}