using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Sucess()
        {
            // arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();        
            // act

            var result = validator.Validate(request);
            
            //assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData(null)]
        public void Error_Name_Empty(string name)
        {
            /// arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();  
            request.Name = name;      
            // act

            var result = validator.Validate(request);
            
            //assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY));
        }

        [Theory]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData(null)]
        public void Error_Email_Empty(string email)
        {
            /// arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();  
            request.Email = email;      
            // act

            var result = validator.Validate(request);
            
            //assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY));
        }
        
        [Fact]
        public void Error_Email_Invalid()
        {
            /// arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();  
            request.Email = "luis.com";      
            // act

            var result = validator.Validate(request);
            
            //assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_INVALID));
        }
        
        
        [Fact]
        public void Error_Password_Empty()
        {
            /// arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();  
            request.Password = string.Empty;      
            // act

            var result = validator.Validate(request);
            
            //assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.INVALID_PASSWORD));
        }
        
    }
}