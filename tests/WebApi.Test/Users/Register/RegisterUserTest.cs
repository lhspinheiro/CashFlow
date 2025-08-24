using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Users.Register;

public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/User";
    private readonly HttpClient _httpClient;
    
    public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
    }
    
    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        
       var result = await _httpClient.PostAsJsonAsync(METHOD,request);
       
       result.StatusCode.Should().Be(HttpStatusCode.Created);
       
       var body = await result.Content.ReadAsStreamAsync();
       
       var response =  await JsonDocument.ParseAsync(body);

       response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
       response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture)); //culture middlewware para enviar na request os idiomas
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var body = await result.Content.ReadAsStreamAsync();
        
        var response =  await JsonDocument.ParseAsync(body);
        
        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture)); //pegando na resource de acorda com o idioma solicitado 
        
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
    
}