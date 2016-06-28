using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Infrastructure.EF.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litskevich.Family.Infrastructure.Services.Repositories
{
    public class GuestRepository : CoreRepository<FamilyDbContext, Guest>, IGuestRepository
    {
        public GuestRepository(FamilyDbContext context)
            : base(context)
        { }

        public IEnumerable<Guest> GetAllByLogin(string login)
        {
            return this.QueryAll
                .Where(g => g.Login.Equals(login, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
