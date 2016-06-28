﻿using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.WebApi;
using SXCore.WebApi.Attributes;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/user")]
    public class UserApiController : CoreApiController
    {
        private IPersonsManager _manager;

        public UserApiController(IPersonsManager manager)
        {
            _manager = manager;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        public void Registration([FromBody]RegistrationRequest request)
        {
            _manager.Registration(request.Name, request.Email, request.Phone);
        }

        [HttpPost]
        [Route("changepassword")]
        public void ChangePassword(long id, [FromBody]ChangePasswordRequest request)
        {
            _manager.ChangePassword(request.PasswordOld, request.PasswordNew);
        }

    }
}
