using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Secutiry.Cryptography;
using CashFlow.Domain.Secutiry.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login
{
    public class DoLoginUSeCase : IDoLoginUSeCase
    {


        private readonly IUserReadOnlyRepository _repository;
        private readonly IpasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;


        public DoLoginUSeCase(IUserReadOnlyRepository repository, IpasswordEncripter passwordEncripter, IAccessTokenGenerator accessTokenGenerator)
        {
            _repository = repository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
        }
        
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJSon request)
        {
            var user = await _repository.GetUserBYEmail(request.Email);

            if (user is null)
            {
                throw new InvalidLoginException();
            }

            var passwordMatch = _passwordEncripter.verify(request.Password, user.Password);

            if(passwordMatch == false)
            {
                throw new InvalidLoginException();
            }


            return new ResponseRegisteredUserJson 
            {
                Name = user.Name,
                Token = _accessTokenGenerator.Generate(user)
            };
        }
    }
}