using System;
using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Domain.Contracts.Repositories;
using Litskevich.Family.Infrastructure.Data;
using Litskevich.Family.Infrastructure.Services.Repositories;
using SXCore.Infrastructure.EF.Services;

namespace Litskevich.Family.Infrastructure.Services
{
    public class FamilyUnitOfWork : CoreUnitOfWork<FamilyDbContext>, IFamilyUnitOfWork
    {
        private IActivityRepository _activityRepository;
        private IPersonRepository _personRepository;
        private IGuestRepository _guestRepository;
        private IArticleRepository _articlesRepository;
        private IMaterialRepository _materialRepository;

        public IActivityRepository ActivityRepository
        {
            get
            {
                if (_activityRepository == null)
                    _activityRepository = new ActivityRepository(this.DbContext);
                return _activityRepository;
            }
        }

        public IPersonRepository PersonRepository
        {
            get
            {
                if (_personRepository == null)
                    _personRepository = new PersonRepository(this.DbContext);
                return _personRepository;
            }
        }

        public IGuestRepository GuestRepository
        {
            get
            {
                if (_guestRepository== null)
                    _guestRepository = new GuestRepository(this.DbContext);
                return _guestRepository;
            }
        }

        public IArticleRepository ArticleRepository
        {
            get
            {
                if (_articlesRepository == null)
                    _articlesRepository = new ArticleRepository(this.DbContext);
                return _articlesRepository;
            }
        }

        public IMaterialRepository MaterialRepository
        {
            get
            {
                if (_materialRepository == null)
                    _materialRepository = new MaterialRepository(this.DbContext);
                return _materialRepository;
            }
        }

        public FamilyUnitOfWork(string connection = "")
            : base(new FamilyDbContext(connection))
        { }

        public FamilyUnitOfWork(FamilyDbContext context)
            : base(context)
        { }
    }
}
