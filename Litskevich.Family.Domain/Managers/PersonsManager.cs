using System;
using System.Collections.Generic;
using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Values;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using System.Linq;
using SXCore.Common.Values;
using SXCore.Common.Classes;
using Litskevich.Family.Domain.Events;
using SXCore.Domain.Entities;
using Litskevich.Family.Domain.Contracts.Services;

namespace Litskevich.Family.Domain.Managers
{
    public class PersonsManager : BaseManager, IPersonsManager
    {
        public PersonsManager(IFamilyUnitOfWork uow, IFamilyInfrastructureProvider infrastructure, ITokenProvider tokenProvider)
            : base(uow, infrastructure, tokenProvider)
        { }

        public Person GetPerson(long id)
        {
            return this.UnitOfWork.PersonRepository.Get(id);
        }

        public IEnumerable<Person> GetPersons(string filter = "")
        {
            return this.UnitOfWork.PersonRepository.FindAll(filter);
        }

        public Person CreatePerson(PersonData data)
        {
            var person = Person.Create(data.Gender, data.Name, data.Email, data.Phone, data.DateBirth, data.DateDeath);

            this.UnitOfWork.PersonRepository.Add(person);

            this.SaveChanges();

            return person;
        }

        public Person UpdatePerson(long personID, PersonData data)
        {
            var person = this.GetPerson(personID);
            if (person == null)
                throw new CustomNotFoundException($"Person with ID={personID} not found");

            person.Change(data.Gender, data.Name, data.Email, data.Phone, data.DateBirth, data.DateDeath);

            this.UnitOfWork.PersonRepository.Update(person);

            this.SaveChanges();

            return person;
        }

        public Person ChangePersonAvatar(long personID, string avatar)
        {
            var person = this.GetPerson(personID);
            if (person == null)
                throw new CustomNotFoundException($"Person with ID={personID} not found");

            if (String.IsNullOrWhiteSpace(avatar))
                return person;

            Avatar avtr = null;
            if (avatar.StartsWith("data:", CommonService.StringComparison))
                avtr = this.SaveAvatar(Imager.CreateFromDataUrl(avatar).Save());
            else
                avtr = this.UnitOfWork.FindByCode<Avatar>(avatar);

            person.ChangeAvatar(avtr);

            this.UnitOfWork.PersonRepository.Update(person);

            this.SaveChanges();

            return person;
        }

        public void Registration(PersonTotalName name, string email, string phone, string comment = "")
        {
            DomainEvents.Raise(new RegistrationRequestedEvent(name, email, phone, comment));
        }

        public void ChangePassword(string passwordOld, string passwordNew)
        {
            var person = this.Token.PersonID <= 0 ? null : this.UnitOfWork.PersonRepository.GetWithManager(this.Token.PersonID);
            if (person == null || person.Manager == null || person.Manager.ID != this.Token.ManagerID)
                throw new CustomArgumentException("Невозможно сменить пароль, так как пользователь не известен!");

            if (!CommonService.VerifyPassword(passwordOld, person.Manager.Password))
                throw new CustomArgumentException("Невозможно сменить пароль, так как указан неверный старый пароль!");

            var pass = String.IsNullOrWhiteSpace(passwordNew) ? CommonService.GeneratePassword(8, false) : passwordNew;

            person.Manager.ChangePassword(pass);

            this.UnitOfWork.PersonRepository.Update(person);

            this.SaveChanges();

            DomainEvents.Raise(new ManagerCreatedEvent(person.Name, person.Email, person.Manager.Login, pass));
        }

        public void CreateManager(long personID, string login, string password = "", string roles = "")
        {
            var person = this.UnitOfWork.PersonRepository.GetWithManager(personID);
            if (person == null)
                throw new CustomNotFoundException($"Person with ID={personID} not found");

            var pass = String.IsNullOrWhiteSpace(password) ? CommonService.GeneratePassword(8, false) : password;

            // create manager roles
            var managerRoles = new List<ManagerRoleType>();
            managerRoles.Add(this.UnitOfWork.FindType<ManagerRoleType>("User"));

            // fill manager roles with selected roles
            if (!String.IsNullOrWhiteSpace(roles))
                foreach (var role in roles.Split(new char[] { ',', ';' }))
                    if (!managerRoles.Any(r => r.Name.Equals(role, CommonService.StringComparison)))
                        managerRoles.Add(this.UnitOfWork.FindType<ManagerRoleType>(role));

            if (person.Manager != null)
            {
                person.Manager.ChangeLogin(login);
                person.Manager.ChangePassword(pass);
                person.Manager.ChangeRoles(managerRoles.ToArray());
            }
            else
            {
                person.CreateManager(login, pass, managerRoles);
            }

            this.UnitOfWork.PersonRepository.Update(person);

            this.SaveChanges();

            DomainEvents.Raise(new ManagerCreatedEvent(person.Name, person.Email, person.Manager.Login, pass));
        }
    }
}
