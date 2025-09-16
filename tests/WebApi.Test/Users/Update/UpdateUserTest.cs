using System.Globalization;
using System.Net;
using System.Text.Json;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Users.Update;

public class UpdateUserTest : CashFlowClassFixture
{
    private const string METHOD = "api/User";
    private readonly string _token;
    
    public UpdateUserTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var result = await DoUpdate(requestUri: METHOD, request: request, _token);
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Name_Empty(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;
        var result = await DoUpdate(requestUri: METHOD, request: request, _token, culture: culture);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var body =  await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);
        
        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));;
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Email_Empty(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = string.Empty;
        var result = await DoUpdate(requestUri: METHOD, request: request, _token, culture: culture);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var body =  await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);
        
        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_EMPTY", new CultureInfo(culture));
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));;
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Email_Invalid(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "luis.com";
        var result = await DoUpdate(requestUri: METHOD, request: request, _token, culture: culture);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var body =  await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);
        
        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_INVALID", new CultureInfo(culture));
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));;
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Email_Exist(string culture)
    {
        var requestRegisterUser = RequestRegisterUserJsonBuilder.Build();
        
        var resultRegister = await DoPost(requestUri: METHOD, request: requestRegisterUser);
        resultRegister.StatusCode.Should().Be(HttpStatusCode.Created);
        var existEmail = requestRegisterUser.Email; 
        
        var requestUpdateUser = RequestUpdateUserJsonBuilder.Build();
        requestUpdateUser.Email = existEmail;
        
        var resultUpdate = await DoUpdate(requestUri: METHOD, request: requestUpdateUser, _token, culture: culture);
        resultUpdate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var body =  await resultUpdate.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);
        
        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_ALREADY_REGISTERED", new CultureInfo(culture));
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));;
    }
    
}