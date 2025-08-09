using CashFlow.Application.UseCases.Users;
using CashFlow.Communication.Requests;
using FluentAssertions;
using FluentValidation;

namespace Validators.Tests.User
{
    public class PasswordValidatorTest
    {
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
        
        public void Error_Password_Invalid(string password)
        {
            /// arrange
            var validator = new PasswordValidator<RequestRegisterUserJson>();
            
            // act

            var result = validator.IsValid(new ValidationContext<RequestRegisterUserJson>(new RequestRegisterUserJson()), password);
            
            //assert
            result.Should().BeFalse();
            
        }
    }
}