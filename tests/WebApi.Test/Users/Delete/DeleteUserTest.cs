using System.Net;
using FluentAssertions;
using Xunit;

namespace WebApi.Test.Users.Delete;

public class DeleteUserTest : CashFlowClassFixture
{
    private const string METHOD = "api/User";
    
    private readonly string _token;


    public DeleteUserTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Sucess()
    {
        var result = await DoDelete(requestUri: METHOD, token: _token);
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        result = await DoGet(requestUri: METHOD, _token);
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}