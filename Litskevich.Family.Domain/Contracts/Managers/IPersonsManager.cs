using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Values;
using SXCore.Common.Values;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Managers
{
    public interface IPersonsManager : ICoreManager
    {
        Person GetPerson(long id);
        IEnumerable<Person> GetPersons(string filter = "");
        Person CreatePerson(PersonData data);
        Person UpdatePerson(long personID, PersonData data);
        Person ChangePersonAvatar(long personID, string avatar);

        void Registration(PersonTotalName name, string email, string phone, string comment = "");
        void ChangePassword(string passwordOld, string passwordNew);
        void CreateManager(long personID, string login, string password = "", string roles = "");

        void PasswordRecoveryInit(string search);
        void PasswordRecoveryComplete(string code);

        void InviteGuest(string nameLast, string nameFirst, string email, string phone, string login, string password = "", int hours = 24);
    }
}
