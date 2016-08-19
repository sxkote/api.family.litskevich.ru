using System.Linq;
using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Infrastructure.EF.Services;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using SXCore.Common;
using SXCore.Common.Services;

namespace Litskevich.Family.Infrastructure.Services.Repositories
{
    public class ActivityRepository : CoreRepository<FamilyDbContext, Activity>, IActivityRepository
    {
        public ActivityRepository(FamilyDbContext dbContext)
            : base(dbContext)
        { }

        public Activity GetByCode(string code)
        {
            return this.QuerySingle.FirstOrDefault(a => a.Code == code);
        }
    }
}
