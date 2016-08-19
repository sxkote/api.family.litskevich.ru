using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IPersonRepository : ICoreRepository<Person>
    {
        IEnumerable<Person> FindAll(string filter = "");
        IEnumerable<Person> GetByIDs(IEnumerable<long> ids);
        Person GetWithManager(long personID);
        Person GetByLogin(string login);
        Person GetByPasswordRecovery(string search);
    }
}
