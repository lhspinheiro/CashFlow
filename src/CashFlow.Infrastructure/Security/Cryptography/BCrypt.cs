using CashFlow.Domain.Secutiry.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace CashFlow.Infrastructure.Security.Cryptography;

public class BCrypt : IpasswordEncripter
{
    public string Encrypt(string password)
    {
        string passwordHash = BC.HashPassword(password);

        return passwordHash;
    }

    public bool verify(string password, string passwordHash) => BC.Verify(password, passwordHash);
    
}
