using Litskevich.Family.Domain.Contracts.Services;
using SXCore.Common.Values;
using System.IO;
using SXCore.Common.Services;
using SXCore.Common.Exceptions;
using Litskevich.Family.Domain.Entities;
using System;

namespace Litskevich.Family.WebApi.App_Start
{
    public class ThumbnailProvider : IThumbnailProvider
    {
        public const string ThumbnailsPath = "~/Content/Images/content-types/";

        private IFamilyInfrastructureProvider _infrastructure;

        public ThumbnailProvider(IFamilyInfrastructureProvider infrastructure)
        {
            _infrastructure = infrastructure;
        }

        public byte[] CreateDefaultThumbnail(FileName.FileType type)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(ThumbnailsPath + GetThumbnailFileName(type));

            return File.ReadAllBytes(path);
        }

        public byte[] CreateImageThumbnail(byte[] image)
        {
            if (image == null)
                throw new CustomArgumentException("Image content can't be empty!");

            var imager = Imager.Create(image);

            return this.CreateImageThumbnail(imager);
        }

        public byte[] CreateImageThumbnail(Imager imager)
        {
            var settings = _infrastructure.GetThumbnailSettings();

            if (imager == null)
                throw new CustomArgumentException("Invalid Imager provided!");

            imager.Modify(ImagerResizeType.Crop, settings.Size);

            return imager.Save(settings.Quality, settings.MimeType);
        }

        public void SaveImageThumbnail(Material material, Imager imager)
        {
            if (_infrastructure.StorageService == null)
                throw new CustomArgumentException("FileStorage Service is not presented!");

            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't save Thumbnail for empty Material!");

            if (imager == null)
                throw new CustomArgumentException("Invalid Imager provided!");

            // get path for thumbnail file
            var thumbnailPath = _infrastructure.GetThumbnailPath(material);

            // thumbnail byte array
            byte[] thumbnailData = this.CreateImageThumbnail(imager);

            _infrastructure.StorageService.SaveFile(thumbnailPath, thumbnailData);
        }

        public void SaveMaterialThumbnail(Material material)
        {
            if (_infrastructure.StorageService == null)
                throw new CustomArgumentException("FileStorage Service is not presented!");

            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't save Thumbnail for empty Material!");

            // get path for thumbnail file
            var thumbnailPath = _infrastructure.GetThumbnailPath(material);

            // thumbnail byte array
            byte[] thumbnailData = null;

            if (material.FileType == FileName.FileType.Image)
            {
                var materialPath = _infrastructure.GetMaterialPath(material);

                var materialData = _infrastructure.StorageService.ReadFile(materialPath);

                if (materialData != null)
                    thumbnailData = this.CreateImageThumbnail(materialData);
            }

            // try to create default thumbnail
            if (thumbnailData == null)
                thumbnailData = this.CreateDefaultThumbnail(material.FileType);

            // save thumbnail byte array to storage
            if (thumbnailData != null)
                _infrastructure.StorageService.SaveFile(thumbnailPath, thumbnailData);
        }

        static public string GetThumbnailFileName(FileName.FileType type)
        {
            switch (type)
            {
                case FileName.FileType.Video:
                case FileName.FileType.Audio:
                    return "Multimedia.png";

                case FileName.FileType.Excel:
                    return "Excel.png";

                case FileName.FileType.Word:
                    return "Word.png";

                case FileName.FileType.PowerPoint:
                    return "PowerPoint.png";

                case FileName.FileType.Text:
                    return "Text.png";

                case FileName.FileType.Image:
                    return "Image.png";

                case FileName.FileType.PDF:
                    return "PDF.png";

                default: return "File.png";
            }
        }
    }
}
