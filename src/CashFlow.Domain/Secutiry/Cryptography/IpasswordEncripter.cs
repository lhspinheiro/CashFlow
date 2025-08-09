namespace CashFlow.Domain.Secutiry.Cryptography;
public interface IpasswordEncripter
{
    string Encrypt(string password);
    bool verify(string password, string passwordHash);
}
