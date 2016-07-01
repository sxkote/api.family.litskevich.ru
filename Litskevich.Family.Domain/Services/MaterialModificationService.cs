using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Events;
using Litskevich.Family.Domain.Values;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Services
{
    public class MaterialModificationService :
        IDomainEventHandler<MaterialAddedEvent>,
        IDomainEventHandler<MaterialFileChangedEvent>,
        IDomainEventHandler<MaterialInfoChangedEvent>,
        IDomainEventHandler<MaterialTransformEvent>,
        IDomainEventHandler<MaterialDeletedEvent>
    {
        protected IFamilyInfrastructureProvider _infrastructure;
        protected IThumbnailProvider _thumbnailProvider;

        public MaterialModificationService(IFamilyInfrastructureProvider infrastructure, IThumbnailProvider thumbnailProvider)
        {
            _infrastructure = infrastructure;
            _thumbnailProvider = thumbnailProvider;
        }

        protected Imager CreateImager(Material material)
        {
            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't create Imager for empty Material!");

            if (_infrastructure.StorageService == null)
                throw new CustomArgumentException("FileStorage Service is not presented!");

            if (material.FileType != FileName.FileType.Image)
                throw new CustomArgumentException("Can't create Imager for not Image Material!");

            // get path for image file
            var materialPath = _infrastructure.GetMaterialPath(material);

            // get image byte array
            var materialData = _infrastructure.StorageService.ReadFile(materialPath);

            // create imager object for image
            return Imager.Create(materialData);
        }

        protected void SaveImager(Material material, Imager imager, int? quality = null)
        {
            if (material == null || material.File == null)
                throw new CustomArgumentException("Can't save Imager for empty Material!");

            if (_infrastructure.StorageService == null)
                throw new CustomArgumentException("FileStorage Service is not presented!");

            if (imager == null)
                throw new CustomArgumentException("Can't save empty Imager!");

            // get path for image file
            var materialPath = _infrastructure.GetMaterialPath(material);

            // save image to byte array
            var materialData = quality == null || quality.Value >= 100 || quality.Value <= 0 ? imager.Save() : imager.Save(quality.Value);

            // save byte array to storage
            _infrastructure.StorageService.SaveFile(materialPath, materialData);

            // change material file info
            material.File.ChangeContentInfo(materialData.Length, CommonService.GetMD5(materialData));
        }

        protected void MoveMaterialToFolder(Material material, string folder)
        {
            // material should be specified
            if (material == null || material.File == null)
                return;

            // storage should be presented
            var storage = _infrastructure.StorageService;
            if (storage == null)
                return;

            // define source paths
            var materialSourcePath = _infrastructure.GetMaterialPath(material);
            var thumbnailSourcePath = _infrastructure.GetThumbnailPath(material);

            // folder path to move
            var moveToFolder = String.IsNullOrWhiteSpace(folder) ? _infrastructure.FolderEmpty : (folder.EndsWith("/") ? folder : folder + "/");

            // changing entity folder value
            material.File.ChangeFolder(moveToFolder);

            // destination file path
            var materialDestPath = _infrastructure.GetMaterialPath(material);
            var thumbnailDestPath = _infrastructure.GetThumbnailPath(material);

            // moving blob to new location
            if (storage.Exist(materialSourcePath))
                storage.CopyFile(materialSourcePath, materialDestPath, true);

            // moving thumbnail to new location
            if (storage.Exist(thumbnailSourcePath))
                storage.CopyFile(thumbnailSourcePath, thumbnailDestPath, true);
        }

        public void Handle(MaterialAddedEvent args)
        {
            // Article should exist
            var article = args.Article;
            if (article == null)
                return;

            // FileBlob should exist
            var material = args.Material;
            if (material == null || material.File == null)
                return;

            this.MoveMaterialToFolder(material, article.Code + "/");
        }

        public void Handle(MaterialFileChangedEvent args)
        {
            // material should be specified
            var material = args?.Material;
            if (material == null || material.File == null)
                return;

            if (material.FileType == FileName.FileType.Image)
            {
                #region Process Image
                var settings = _infrastructure.GetImageSettings();

                // create Imager 
                var imager = this.CreateImager(material);

                // get Image Info
                var info = imager.Info;

                // decide if the image should be resized (file size is big || dimensions are too large)
                var resize = settings.ResizeLargeImages && material.File.Size > settings.MaxFileSize && (imager.Width > settings.MaxWidth || imager.Height > settings.MaxHeight);
                if (resize)
                {
                    // resize the image
                    imager.Resize(settings.MaxWidth, settings.MaxHeight);
                }

                // rotate correction
                imager.RotateCorrection();

                // save updated image
                this.SaveImager(material, imager, settings.Quality);

                // save thumbnail for image
                _thumbnailProvider.SaveImageThumbnail(material, imager);

                // if there is any info about image, than we could update material info
                if (info != null && !String.IsNullOrWhiteSpace(info.Title) || !String.IsNullOrWhiteSpace(info.Comment) || info.Date != null)
                    material.ChangeInfo(info.Date, info.Title, info.Comment);
                #endregion
            }
            else
            {
                _thumbnailProvider.SaveMaterialThumbnail(material);
            }
        }

        public void Handle(MaterialTransformEvent args)
        {
            // material should be specified
            var material = args?.Material;
            if (material == null || material.File == null)
                return;

            if (material.FileType == FileName.FileType.Image)
            {
                #region Process Image Transformation
                var imager = this.CreateImager(material);
                var isModified = false;

                if (args.Method.Equals("rotate", CommonService.StringComparison))
                {
                    imager.Rotate(args.Argument.Equals("true", CommonService.StringComparison));
                    isModified = true;
                }

                if (isModified)
                {
                    // resave modified image
                    this.SaveImager(material, imager);

                    // resave thumbnail that should be changed
                    _thumbnailProvider.SaveImageThumbnail(material, imager);
                }
                #endregion
            }
        }

        public void Handle(MaterialInfoChangedEvent args)
        {
            // material should be specified
            var material = args?.Material;
            if (material == null || material.File == null)
                return;

            if (material.FileType == FileName.FileType.Image)
            {
                // create imager object for image file
                var imager = this.CreateImager(material);

                // if any info about image has changed, than we need to update image file
                if (!imager.Info.Title.Equals(material.Title, StringComparison.InvariantCultureIgnoreCase)
                    || !imager.Info.Comment.Equals(material.Comment, StringComparison.InvariantCultureIgnoreCase)
                    || imager.Info.Date != material.Date)
                {
                    imager.Info.Title = material.Title;
                    imager.Info.Comment = material.Comment;
                    imager.Info.Date = material.Date;

                    this.SaveImager(material, imager);
                }
            }
        }

        public void Handle(MaterialDeletedEvent args)
        {
            // Article should exist
            var article = args.Article;
            if (article == null)
                return;

            // FileBlob should exist
            var material = args.Material;
            if (material == null || material.File == null)
                return;

            // delete Thumbnail
            var thumbnailPath = _infrastructure.GetThumbnailPath(material);
            if (_infrastructure.StorageService.Exist(thumbnailPath))
                _infrastructure.StorageService.DeleteFile(thumbnailPath);

            this.MoveMaterialToFolder(material, _infrastructure.FolderDeleted);
        }
    }
}
