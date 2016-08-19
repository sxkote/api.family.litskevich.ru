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

        public void InviteGuest(string nameLast, string nameFirst, string email, string phone, string login, string password = "", int hours = 24)
        {
            if (this.CurrentPerson == null)
                throw new CustomArgumentException("Person is undefined!");

            if (String.IsNullOrEmpty(login))
                throw new CustomArgumentException("Login can't be empty for new Guest!");

            var now = CommonService.Now;

            var oldGuestsWithSameLogin = this.UnitOfWork.GuestRepository.GetAllByLogin(login);
            foreach (var oldGuest in oldGuestsWithSameLogin)
            {
                if (oldGuest.Expire > now)
                    oldGuest.Disable();
                this.UnitOfWork.GuestRepository.Update(oldGuest);
            }

            var pass = String.IsNullOrWhiteSpace(password) ? CommonService.GeneratePassword(8, false) : password;

            var guest = Guest.Create(this.CurrentPerson, new PersonName(nameFirst ?? "", nameLast ?? ""), email, phone, now.AddHours(hours), login, pass);

            this.UnitOfWork.GuestRepository.Add(guest);


            this.SaveChanges();

            DomainEvents.Raise(new GuestCreatedEvent(guest.Name, guest.Email, guest.Login, pass));
        }

        public void PasswordRecoveryInit(string search)
        {
            var person = this.UnitOfWork.PersonRepository.GetByPasswordRecovery(search);
            if (person == null || person.Manager == null)
                throw new CustomNotFoundException("Пользователь не найден!");

            var activity = Activity.Create(CommonService.Now.AddDays(1), "PasswordRecovery", person.ID.ToString());

            this.UnitOfWork.ActivityRepository.Add(activity);

            this.SaveChanges();

            DomainEvents.Raise(new PasswordRecoveryRequestedEvent(activity.Code, person.Name, person.Email));
        }

        public void PasswordRecoveryComplete(string code)
        {
            var errorMessage = "Невозможно восстановить пароль! Попробуйте еще раз!";

            try
            {
                // get valid activity
                var activity = this.UnitOfWork.ActivityRepository.GetByCode(code);
                if (activity == null || !activity.IsValid())
                    throw new CustomOperationException(errorMessage);

                // check if activity of type RestorePassword
                if (!activity.Identifier.Equals("PasswordRecovery", StringComparison.OrdinalIgnoreCase))
                    throw new CustomOperationException(errorMessage);

                // check if Data is not empty
                if (String.IsNullOrWhiteSpace(activity.Data))
                    throw new CustomOperationException(errorMessage);

                // get Person by ID (stored in the Data field of activity)
                var personID = Convert.ToInt64(activity.Data);
                var person = this.UnitOfWork.PersonRepository.GetWithManager(personID);
                if (person == null || person.Manager == null)
                    throw new CustomOperationException(errorMessage);

                // generate new password
                var pass = CommonService.GeneratePassword(8, false);

                // change password for manager
                person.Manager.ChangePassword(pass);
                this.UnitOfWork.PersonRepository.Update(person);

                // discard activity to futher actions
                activity.Discard();
                this.UnitOfWork.ActivityRepository.Update(activity);

                this.SaveChanges();

                // password was changed
                DomainEvents.Raise(new PasswordChangedEvent(person.Name, person.Email, person.Manager.Login, pass));
            }
            catch
            {
                throw new CustomOperationException(errorMessage);
            }
        }
    }
}
