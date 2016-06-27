using System.Linq;
using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Infrastructure.EF.Services;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using SXCore.Common;

namespace Litskevich.Family.Infrastructure.Services.Repositories
{
    public class ArticleRepository : CoreRepository<FamilyDbContext, Article>, IArticleRepository
    {
        public ArticleRepository(FamilyDbContext dbContext)
            : base(dbContext)
        { }

        protected override IQueryable<Article> Query
        {
            get
            {
                return base.Query
                    .Include(t => t.Author)
                    .Include(t => t.Members.Select(p => p.Avatar));
            }
        }

        public Article GetWithMaterials(long articleID)
        {
            return this.QuerySingle
                .Include(t => t.Materials.Select(m => m.File))
                .FirstOrDefault(a => a.ID == articleID);
        }

        public IEnumerable<Article> FindAll(string filter = "", int pageNumber = 0, int pageSize = Article.DefaultPageSize)
        {
            var query = this.QueryAll;

            if (!String.IsNullOrEmpty(filter))
            {
                var fullFilter = filter.Contains('%') ? filter : String.Format("%{0}%", filter);
                query = query.Where(p => System.Data.Entity.SqlServer.SqlFunctions.PatIndex(fullFilter, p.Title + " " + p.Comment) > 0);
            }

            return query
                .OrderByDescending(a => a.Date)
                .Page(pageNumber, pageSize)
                .ToList();
        }

        public IEnumerable<Article> FindByPerson(long personID, int pageNumber = 0, int pageSize = Article.DefaultPageSize)
        {
            return this.QueryAll
                .Where(t => t.Members.Any(p => p.ID == personID))
                .OrderByDescending(a => a.Date)
                .Page(pageNumber, pageSize)
                .ToList();
        }
    }
}
