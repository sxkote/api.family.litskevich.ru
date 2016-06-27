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
        private IPersonRepository _personRepository;
        private IArticleRepository _articlesRepository;
        private IMaterialRepository _materialRepository;

        public IPersonRepository PersonRepository
        {
            get
            {
                if (_personRepository == null)
                    _personRepository = new PersonRepository(this.DbContext);
                return _personRepository;
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
