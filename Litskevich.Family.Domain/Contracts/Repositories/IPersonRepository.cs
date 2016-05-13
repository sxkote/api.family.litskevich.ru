using Litskevich.Family.Domain.Entities;
using SXCore.Common.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IPersonRepository : ICoreRepository<Person>
    {
        IEnumerable<Person> FindAll(string filter = "");
        //Person SearchByLogin(string login);
    }
}
