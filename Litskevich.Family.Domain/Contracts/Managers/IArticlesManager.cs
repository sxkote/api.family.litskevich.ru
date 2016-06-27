using Litskevich.Family.Domain.Entities;
using SXCore.Common.Values;
using SXCore.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Contracts.Managers
{
    public interface IArticlesManager : ICoreManager
    {
        Article GetArticle(long id);
        Article GetArticleWithMaterials(long id);

        IEnumerable<Article> GetArticles(string filter, int pageNumber, int pageSize);
        IEnumerable<Article> GetArticles(long personID, int pageNumber, int pageSize);

        Article CreateArticle(string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "");
        Article UpdateArticle(long articleID, string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "", long[] personIDs = null);
    }
}
