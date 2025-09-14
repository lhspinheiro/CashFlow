using System.Net;
using System.Net.Mime;
using FluentAssertions;
using Xunit;

namespace WebApi.Test.Expenses.Reports;

public class GenerateExpensesReportTest : CashFlowClassFixture
{
    private const string METHOD = "api/Report";
    private readonly string _adminToken;
    private readonly string _teamMemberToken;
    private readonly DateTime _expensesDate;

    public GenerateExpensesReportTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _adminToken = webApplicationFactory.User_Admin.GetToken();
        _teamMemberToken = webApplicationFactory.User_Team_Member.GetToken();
        _expensesDate = webApplicationFactory.Expense_Admin.GetDate();
    }

    [Fact]
    public async Task Sucess_Pdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expensesDate: yyyy-MM}", token: _adminToken);
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Pdf);
    }
    
    [Fact]
    public async Task Sucess_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expensesDate: yyyy-MM}", token: _adminToken);
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Octet);
    }

    [Fact]
    public async Task Error_forbidden_User_Not_Allowed_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expensesDate: Y}", token: _teamMemberToken);
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Error_forbidden_User_Not_Allowed_Pdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expensesDate: Y}", token: _teamMemberToken);
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
}