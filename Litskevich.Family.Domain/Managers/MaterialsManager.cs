using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Events;
using SXCore.Common.Classes;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Managers
{
    public class MaterialsManager : BaseManager, IMaterialsManager
    {
        public MaterialsManager(IFamilyUnitOfWork uow, IFamilyInfrastructureProvider infrastructure, ITokenProvider tokenProvider)
            : base(uow, infrastructure, tokenProvider)
        { }

        public Material GetMaterial(long id)
        {
            return this.UnitOfWork.MaterialRepository.Get(id);
        }

        public Material CreateMaterial(long articleID, string fileCode)
        {
            var article = this.UnitOfWork.ArticleRepository.Get(articleID);
            if (article == null)
                throw new CustomNotFoundException($"Article with ID={articleID} not found!");

            var blob = this.UnitOfWork.FindByCode<FileBlob>(fileCode);
            if (blob == null)
                throw new CustomNotFoundException($"Blob with Code={fileCode} not found!");

            var material = article.AddMaterial(blob);

            this.UnitOfWork.MaterialRepository.Add(material);
            this.UnitOfWork.ArticleRepository.Update(article);

            this.SaveChanges();

            DomainEvents.Raise(new MaterialSavedEvent(material));

            return material;
        }

        public Material UpdateMaterial(long materialID, DateTime? date = null, string title = "", string comment = "")
        {
            var material = this.GetMaterial(materialID);
            if (material == null)
                throw new CustomNotFoundException($"Material with ID={materialID} not found!");

            material.ChangeInfo(date, title, comment);

            this.UnitOfWork.MaterialRepository.Update(material);

            this.SaveChanges();

            return material;
        }

        public void DeleteMaterial(long materialID)
        {
            var material = this.GetMaterial(materialID);
            if (material == null)
                throw new CustomNotFoundException($"Material with ID={materialID} not found!");

            var article = material.Article;

            var deleted = article.RemoveMaterial(materialID);
            if (deleted == null)
                return;

            ((IDbEntity)deleted).DeleteFromDb();

            this.UnitOfWork.ArticleRepository.Update(article);
            this.UnitOfWork.MaterialRepository.Update(material);

            this.SaveChanges();
        }

        public void TransformMaterial(long materialID, string method, string argument)
        {
            var material = this.GetMaterial(materialID);
            if (material == null || material.File == null)
                throw new CustomNotFoundException($"Material with ID={materialID} not found!");

            DomainEvents.Raise(new MaterialTransformEvent(material, method, argument));
        }

        public Material ImportMaterial(Article article, string filename, byte[] data, DateTime? date = null, string title = "", string comment = "")
        {
            if (article == null)
                throw new CustomNotFoundException("Invalid Article specified!");

            var blob = FileBlob.Create(article.Code + "/", filename, CommonService.GetMD5(data), data.Length);

            //this.UnitOfWork.Create(blob);

            this.FileStorageService.SaveFile(blob.GetPath(), data);

            var material = article.AddMaterial(blob);

            material.ChangeInfo(date, title, comment);

            this.UnitOfWork.ArticleRepository.Update(article);
            this.UnitOfWork.MaterialRepository.Add(material);

            this.SaveChanges();

            return material;
        }
    }
}
