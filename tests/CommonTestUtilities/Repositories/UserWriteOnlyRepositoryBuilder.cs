using CashFlow.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UserWriteOnlyRepositoryBuilder
{
    public static IUserWriteOnlyRepository Builder()
    {
        var mock = new Mock<IUserWriteOnlyRepository>();
        
        return mock.Object;
    }
}