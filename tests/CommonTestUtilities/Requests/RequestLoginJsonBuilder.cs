using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestLoginJsonBuilder
{
    public static RequestLoginJSon Build()
    {
        return new Faker<RequestLoginJSon>()
            .RuleFor(user => user.Email, faker => faker.Internet.Email())
            .RuleFor(user => user.Password, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
}