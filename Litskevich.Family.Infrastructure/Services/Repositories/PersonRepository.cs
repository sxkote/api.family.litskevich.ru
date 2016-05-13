using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Common.Services;
using SXCore.Infrastructure.EF.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Litskevich.Family.Infrastructure.Services.Repositories
{
    public class PersonRepository : CoreRepository<FamilyDbContext, Person>, IPersonRepository
    {
        public PersonRepository(FamilyDbContext context)
            : base(context)
        { }

        protected override IQueryable<Person> Query
        {
            get
            {
                return base.Query
                    .Include(t => t.Avatar);
            }
        }

        public IEnumerable<Person> FindAll(string filter = "")
        {
            var query = this.QueryAll;

            if (!String.IsNullOrEmpty(filter))
            {
                var fullFilter = filter.Contains('%') ? filter : String.Format("%{0}%", filter);
                query = query.Where(p => System.Data.Entity.SqlServer.SqlFunctions.PatIndex(fullFilter, p.Name.Last + " " + p.Name.First + " " + p.Name.Second + " " + p.Name.Maiden) > 0);
            }

            return query.ToList();
        }

        public Person SearchByLogin(string login)
        {
            var result = this.QuerySingle
                .Include(p => p.Manager)
                .SingleOrDefault(p => p.Manager.Login.Equals(login, CommonService.StringComparison));

            if (result != null)
                return result;

            //return this.QuerySingle
            //    .Include(p => p.Manager)
            //    .SingleOrDefault(p => p.Email.Equals(login, CommonService.StringComparison) || p.Phone.Equals(login, CommonService.StringComparison));

            return null;
        }
    }
}
