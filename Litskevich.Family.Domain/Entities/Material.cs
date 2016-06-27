using SXCore.Domain.Interfaces;
using System;
using SXCore.Domain.Entities;
using SXCore.Common.Exceptions;
using SXCore.Common.Classes;
using Litskevich.Family.Domain.Events;

namespace Litskevich.Family.Domain.Entities
{
    public class Material : Entity, IEntityWithFile
    {
        public Article Article { get; private set; }
        public FileBlob File { get; private set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public SXCore.Common.Values.FileName.FileType FileType
        {
            get
            {
                return this.File?.FileName == null ? SXCore.Common.Values.FileName.FileType.File : this.File.FileName.Type;
            }
        }

        private Material() { }

        public void ChangeFile(FileBlob file)
        {
            if (file == null)
                throw new CustomArgumentException("FileBlob should be specified for Material!");

            this.File = file;

            DomainEvents.Raise(new MaterialFileChangedEvent(this));
        }

        public void ChangeInfo(DateTime? date, string title, string comment)
        {
            this.Date = date;
            this.Title = title ?? "";
            this.Comment = comment ?? "";

            DomainEvents.Raise(new MaterialInfoChangedEvent(this));
        }

        public string GetPath()
        {
            if (this.File == null)
                throw new CustomArgumentException("FileBlob is not defined within Material!");

            return this.File.GetPath();
        }
        
        static public Material Create(Article article, FileBlob fileblob)
        {
            if (article == null)
                throw new CustomArgumentException("Invalid Article was specified to create Material!");

            if (fileblob == null)
                throw new CustomArgumentException("FileBlob should be specified for Material!");

            var material = new Material()
            {
                Article = article
            };

            material.ChangeFile(fileblob);

            return material;
        }
    }
}
