using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestChangePasswordJSonBuilder
{
    public static RequestChangePasswordJSon Build()
    {
        return new Faker<RequestChangePasswordJSon>()
            .RuleFor(user => user.Password, faker => faker.Internet.Password())
            .RuleFor(user => user.NewPassword, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
    
}