using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IMaterialRepository : ICoreRepository<Material>
    {
        IEnumerable<Material> GetAllByArticle(long articleID);
    }
}
