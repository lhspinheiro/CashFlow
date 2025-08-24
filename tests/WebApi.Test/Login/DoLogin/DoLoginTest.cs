using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Login.DoLogin;

public class DoLoginTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Login";
    private readonly HttpClient _httpClientclient;
    private readonly string _email;
    private readonly string _name;
    private readonly string _password;
    


    public DoLoginTest(CustomWebApplicationFactory customWebApplicationFactory)
    {
        _httpClientclient = customWebApplicationFactory.CreateClient();
        _email = customWebApplicationFactory.GetEmail();
        _name = customWebApplicationFactory.GetName();
        _password = customWebApplicationFactory.GetPassword();
        
    }

    [Fact]
    public async Task Sucess()
    {
        var request = new RequestLoginJSon
        {
            Email = _email,
            Password = _password
        };

        var response = await _httpClientclient.PostAsJsonAsync(METHOD, request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("name").GetString().Should().Be(_name);
        responseData.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }


}