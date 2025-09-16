using CashFlow.Application.UseCases.Users.ChangePassword;
using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.User.ChangePassword;

public class ChangePasswordValidatorTest
{
    
    [Fact]
    public void Sucess()
    {
        var validator = new ChangePasswordValidator();
        var request = RequestChangePasswordJSonBuilder.Build();
        
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("aaa")]
    [InlineData("aaaa")]
    [InlineData("aaaaa")]
    [InlineData("aaaaaa")]
    [InlineData("aaaaaaa")]
    [InlineData("aaaaaaaa")]
    [InlineData("aaaaaaaaa")]
    [InlineData("AAAAAAAA")]
    [InlineData("Aaaaaaaaa")]
    [InlineData("Aaaaaaaa1")]
    public void Error_Password_Invalid(string newPassword)
    {
        var validator = new ChangePasswordValidator();
        var request = RequestChangePasswordJSonBuilder.Build();
        request.NewPassword = newPassword;
        
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.INVALID_PASSWORD));
    }

}