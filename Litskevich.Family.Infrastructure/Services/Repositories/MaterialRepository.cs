using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Infrastructure.EF.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Litskevich.Family.Infrastructure.Services.Repositories
{
    public class MaterialRepository : CoreRepository<FamilyDbContext, Material>, IMaterialRepository
    {
        public MaterialRepository(FamilyDbContext dbContext)
            : base(dbContext)
        { }

        protected override IQueryable<Material> Query
        {
            get
            {
                return base.Query
                    .Include(m => m.Article)
                    .Include(m => m.File);
            }
        }

        public IEnumerable<Material> GetAllByArticle(long articleID)
        {
            return this.QueryAll.Where(m => m.Article.ID == articleID)
                .OrderBy(m => m.Date)
                .ThenBy(m => m.ID)
                .ToList();
        }
    }
}
