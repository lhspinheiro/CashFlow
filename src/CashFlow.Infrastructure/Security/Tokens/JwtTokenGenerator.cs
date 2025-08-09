using CashFlow.Domain.Entities;
using CashFlow.Domain.Secutiry.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CashFlow.Infrastructure.Security.Tokens;

public class JwtTokenGenerator : IAccessTokenGenerator
{

    private readonly uint _expirationTimeMinutes;
    private readonly string _signingKey;
    public JwtTokenGenerator(uint expirationTimeMinutes, string signingKey)
    {
        _expirationTimeMinutes = expirationTimeMinutes;
        _signingKey = signingKey;
    }

    public string Generate(User user)
    {

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Sid, user.UserIdentifier.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes), //essa propriedades vai dizer quando o token vai expirar 
            SigningCredentials = new SigningCredentials(SecurityKey(), SecurityAlgorithms.HmacSha256), //chave que utilizará 
            Subject = new ClaimsIdentity(claims)// vamos passar as propriedades desejadas no token.
        };

        var tokenHandler = new JwtSecurityTokenHandler(); 
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken); //devolver o token em string
    }


    private SymmetricSecurityKey SecurityKey()
    {
        var key = Encoding.UTF8.GetBytes(_signingKey);

        return new SymmetricSecurityKey(key);
    }
}
