using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Services;
using Litskevich.Family.Infrastructure.Services;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Values;
using SXCore.Infrastructure.Services;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Infrastructure.Values;
using SXCore.WebApi;
using System;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthApiController : CoreApiController
    {
        private IFamilyInfrastructureProvider _infrastructure;
        private IAuthenticationProvider _authProvider;

        public AuthApiController(IFamilyInfrastructureProvider infrastructure, IAuthenticationProvider authProvider)
        {
            _authProvider = authProvider;
            _infrastructure = infrastructure;
        }

        //[HttpGet]
        //[Route("{login}/{password}")]
        //public Token Authenticate(string login, string password)
        //{
        //    var token = _authProvider.Authenticate(login, password);
        //    if (token == null)
        //        throw new CustomAuthenticationException(login);

        //    return token;
        //}

        [HttpPost]
        [Route("")]
        public Token Authenticate([FromBody]AuthenticationRequest request)
        {
            var token = _authProvider.Authenticate(request.Login, request.Password);
            if (token == null)
                throw new CustomAuthenticationException(request.Login);

            var sasToken = _infrastructure.CreateStorageSASToken(4);
            if (sasToken != null)
                token.Values.Add(sasToken);

            return token;
        }
    }
}
