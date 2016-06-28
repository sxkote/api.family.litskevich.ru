using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Values;
using SXCore.Common.Contracts;
using SXCore.Common.Values;

namespace Litskevich.Family.Domain.Contracts.Services
{
    public interface IFamilyInfrastructureProvider : IInfrastructureProvider
    {
        string WebSiteUrl { get; }

        string FolderEmpty { get; }
        string FolderDeleted { get; }

        StorageSettings GetStorageSettings();
        ImageSettings GetImageSettings();
        ThumbnailSettings GetThumbnailSettings();

        string GetMaterialPath(Material material);
        string GetThumbnailPath(Material material);

        ParamValue CreateStorageSASToken(int hours);
    }
}
