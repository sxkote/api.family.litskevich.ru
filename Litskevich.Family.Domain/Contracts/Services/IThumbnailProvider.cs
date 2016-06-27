using Litskevich.Family.Domain.Entities;
using SXCore.Common.Services;
using SXCore.Common.Values;

namespace Litskevich.Family.Domain.Contracts.Services
{
    public interface IThumbnailProvider
    {
        byte[] CreateImageThumbnail(byte[] image);
        byte[] CreateImageThumbnail(Imager imager);
        byte[] CreateDefaultThumbnail(FileName.FileType type);
        void SaveImageThumbnail(Material material, Imager imager);
        void SaveMaterialThumbnail(Material material);
    }
}
