using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IGuestRepository : ICoreRepository<Guest>
    {
        IEnumerable<Guest> GetAllByLogin(string login);
    }
}
