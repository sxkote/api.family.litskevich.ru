using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IActivityRepository : ICoreRepository<Activity>
    {
        Activity GetByCode(string code);
    }
}
