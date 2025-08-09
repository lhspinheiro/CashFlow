using CashFlow.Domain.Secutiry.Cryptography;
using CommonTestUtilities.Entities;
using Moq;

namespace CommonTestUtilities.Cryptography;

public class PasswordEncrypterBuilder
{

    private readonly Mock<IpasswordEncripter> _mock;

    public PasswordEncrypterBuilder()
    {
        _mock = new Mock<IpasswordEncripter>();
        _mock.Setup(encrypter => encrypter.Encrypt(It.IsAny<string>())).Returns("!@#$dsf234");
    }

    public PasswordEncrypterBuilder Verify(string? password)
    {
        if (string.IsNullOrEmpty(password) == false)
        {
            _mock.Setup(passwordEncrypter => passwordEncrypter.verify(password, It.IsAny<string>())).Returns(true);
        }
        
        return this;
    }
    public IpasswordEncripter Build() => _mock.Object;
}