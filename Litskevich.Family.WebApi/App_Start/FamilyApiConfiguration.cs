using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Events;
using Litskevich.Family.Domain.Managers;
using Litskevich.Family.Domain.Services;
using Litskevich.Family.Infrastructure.Services;
using SXCore.Common.Contracts;
using SXCore.Common.Enums;
using SXCore.Common.Interfaces;
using SXCore.WebApi;

namespace Litskevich.Family.WebApi
{
    public class FamilyApiConfiguration : ApiConfiguration
    {
        public override void Config(IDependencyRegistrator container, DependencyScope scope = DefaultDependencyScope)
        {
            // Unit of Work
            container.RegisterType<FamilyUnitOfWork, IFamilyUnitOfWork>(scope);

            // Managers
            container.RegisterType<FilesManager, IFilesManager>(scope);
            container.RegisterType<PersonsManager, IPersonsManager>(scope);
            container.RegisterType<ArticlesManager, IArticlesManager>(scope);
            container.RegisterType<MaterialsManager, IMaterialsManager>(scope);

            // Services


            // Domain Events Handlers
            container.RegisterType<NotificationsHandlerService, IDomainEventHandler<ManagerCreatedEvent>>(scope);
            container.RegisterType<NotificationsHandlerService, IDomainEventHandler<RegistrationRequestedEvent>>(scope);
            container.RegisterType<MaterialModificationService, IDomainEventHandler<MaterialAddedEvent>>(scope);
            container.RegisterType<MaterialModificationService, IDomainEventHandler<MaterialDeletedEvent>>(scope);
            container.RegisterType<MaterialModificationService, IDomainEventHandler<MaterialFileChangedEvent>>(scope);
            container.RegisterType<MaterialModificationService, IDomainEventHandler<MaterialTransformEvent>>(scope);
            container.RegisterType<MaterialModificationService, IDomainEventHandler<MaterialInfoChangedEvent>>(scope);
        }
    }
}
