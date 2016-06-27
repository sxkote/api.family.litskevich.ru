using Litskevich.Family.Domain.Contracts;
using SXCore.Common.Contracts;
using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Managers;
using Litskevich.Family.Domain.Contracts.Services;

namespace Litskevich.Family.Domain.Managers
{
    public class BaseManager : CoreManager<IFamilyUnitOfWork>
    {
        protected IFamilyInfrastructureProvider _infrastructure;
        protected ITokenProvider _tokenProvider;

        public BaseManager(IFamilyUnitOfWork uow, IFamilyInfrastructureProvider infrastructure, ITokenProvider tokenProvider)
            : base(uow)
        {
            _infrastructure = infrastructure;
            _tokenProvider = tokenProvider;
        }

        protected IFamilyInfrastructureProvider Infrastructure { get { return _infrastructure; } }

        protected override IFileStorageService FileStorageService
        {
            get
            {
                return _infrastructure?.StorageService;
            }
        }

        protected override ITokenProvider TokenProvider
        {
            get
            {
                return _tokenProvider;
            }
        }

        protected Person CurrentPerson
        {
            get
            {
                var token = this.Token;
                if (token == null || token.PersonID <= 0)
                    return null;

                return this.UnitOfWork.FindByID<Person>(token.PersonID);
            }
        }
    }
}
