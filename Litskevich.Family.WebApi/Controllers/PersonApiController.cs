using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Values;
using Litskevich.Family.WebApi.Models;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.WebApi;
using SXCore.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/person")]
    public class PersonApiController : CoreApiController
    {
        private IPersonsManager _manager;

        public PersonApiController(IPersonsManager manager)
        {
            _manager = manager;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IEnumerable<PersonModel> GetAll(string filter = "")
        {
            return _manager.GetPersons(filter).Select(p => p.ToSerial());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id:long}")]
        public PersonModel Get(long id)
        {
            var person = _manager.GetPerson(id);
            if (person == null)
                throw new CustomNotFoundException($"Person with ID={id} not found!");

            return person.ToSerial();
        }

        [ApiAuthorize(Roles = "Supervisor;Admin")]
        [HttpPost]
        [Route("")]
        public PersonModel CreatePerson([FromBody]CreatePersonRequest request)
        {
            var data = new PersonData()
            {
                Name = request.Name,
                Gender = SXCore.Common.Enums.Gender.Male,
                DateBirth = null,
                DateDeath = null
            };

            return _manager.CreatePerson(data).ToSerial();
        }

        [ApiAuthorize(Roles = "Supervisor,Admin")]
        [HttpPost]
        [Route("{id:long}")]
        public PersonModel UpdatePerson(long id, [FromBody]UpdatePersonRequest request)
        {
            var data = new PersonData()
            {
                Name = request.Name,
                Gender = request.Gender,
                Email = request.Email,
                Phone = request.Phone,
                DateBirth = request.DateBirth,
                DateDeath = request.DateDeath
            };

            if (!String.IsNullOrWhiteSpace(request.Avatar))
                _manager.ChangePersonAvatar(id, request.Avatar);

            return _manager.UpdatePerson(id, data).ToSerial();
        }

        [ApiAuthorize(Roles = "Admin")]
        [HttpPost]
        [Route("{id:long}/manager")]
        public void CreateManager(long id, [FromBody]CreateManagerRequest request)
        {
            _manager.CreateManager(id, request.Login, request.Password, request.Roles);
        }
    }
}
