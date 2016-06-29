using Litskevich.Family.Domain.Contracts.Services;
using SXCore.Infrastructure.Services;
using System;
using Litskevich.Family.Domain.Entities;
using SXCore.Common.Exceptions;
using Litskevich.Family.Domain.Values;
using SXCore.Infrastructure.Values;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Common.Values;

namespace Litskevich.Family.Infrastructure.Services
{
    public class FamilyInfrastructureProvider : InfrastructureProvider, IFamilyInfrastructureProvider
    {
        public string MainEmail { get { return this.GetSettings("MainEmail"); } }
        public string WebSiteUrl { get { return this.GetSettings("WebSiteUrl"); } }

        public string FolderEmpty { get { return this.GetStorageSettings().FolderEmpty; } }
        public string FolderDeleted { get { return this.GetStorageSettings().FolderDeleted; } }

        protected StorageSettings _storageSettings;
        protected ImageSettings _imageSettings;
        protected ThumbnailSettings _thumbnailSettings;

        public FamilyInfrastructureProvider()
            : base()
        { }

        public StorageSettings GetStorageSettings()
        {
            if (_storageSettings == null)
                _storageSettings =  this.GetSettings<StorageSettings>("StorageSettings") ?? StorageSettings.Default;
            return _storageSettings;
        }

        public ImageSettings GetImageSettings()
        {
            if (_imageSettings == null)
                _imageSettings = this.GetSettings<ImageSettings>("ImageSettings") ?? ImageSettings.Default;
            return _imageSettings;
        }

        public ThumbnailSettings GetThumbnailSettings()
        {
            if (_thumbnailSettings == null)
                _thumbnailSettings = this.GetSettings<ThumbnailSettings>("ThumbnailSettings") ?? ThumbnailSettings.Default;
            return _thumbnailSettings;
        }

        public string GetMaterialPath(Material material)
        {
            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't define path for empty Material!");

            return material.GetPath();
        }

        public string GetThumbnailPath(Material material)
        {
            var settings = this.GetThumbnailSettings();

            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't define Thumbnail path for empty Material!");

            return material.GetPath() + settings.Suffix;
        }

        public ParamValue CreateStorageSASToken(int hours)
        {
            try
            {
                // add Azure SAS Token Info to get direct to material urls
                var storageConfig = this.GetSettings<FileStorageConfig>(StorageConfigSettingsName);
                if (storageConfig != null && storageConfig.Type == FileStorageConfig.StorageType.Azure && !String.IsNullOrWhiteSpace(storageConfig.ConnectionString))
                    return new ParamValue(this.GetStorageSettings().SASTokenName, AzureFileStorageService.CreateAzureSASToken(storageConfig.ConnectionString, hours));
            }
            catch { }

            return null;
        }
    }
}
