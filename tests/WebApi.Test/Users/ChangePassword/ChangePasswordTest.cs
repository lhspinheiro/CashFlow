using System.Globalization;
using System.Net;
using System.Text.Json;
using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Users.ChangePassword;

public class ChangePasswordTest : CashFlowClassFixture
{
    private const string METHOD = "api/User/change-password";
    
    private readonly string _token;
    private readonly string _password;
    private readonly string _email;

    public ChangePasswordTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _password = webApplicationFactory.User_Team_Member.GetPassword();
        _email = webApplicationFactory.User_Team_Member.GetEmail();
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestChangePasswordJSonBuilder.Build();
        request.Password = _password;
        
        var response = await DoUpdate(requestUri: METHOD, request: request, token:  _token);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var loginREquest = new RequestLoginJSon //garantir que a senha foi trocada
        {
            Email = _email,
            Password = _password,
        };
        
        response = await DoPost(requestUri: "api/Login", request: loginREquest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        loginREquest.Password = request.NewPassword;
        
        response = await DoPost(requestUri: "api/Login", request: loginREquest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Password_Different_Current_Password(string culture)
    {
        var request = RequestChangePasswordJSonBuilder.Build();
        var response = await DoUpdate(requestUri: METHOD, request: request, token:  _token, culture: culture);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        await using var repositoryBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(repositoryBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("PASSWORD_DIFFERENT_CURRENT_PASSWORD", new CultureInfo(culture));
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));;
        
    }
}