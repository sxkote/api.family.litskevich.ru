using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Repositories
{
    public interface IArticleRepository : ICoreRepository<Article>
    {
        Article GetWithMaterials(long articleID);
        IEnumerable<Article> FindAll(string filter, int pageNumber, int pageSize);
        IEnumerable<Article> FindByPerson(long personID, int pageNumber, int pageSize);
    }
}
