using SXCore.Common.Entities;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class Material : Entity, IEntityWithFile
    {
        public DateTimeOffset? Date { get; private set; }
        public string Title { get; private set; }
        public string Comment { get; private set; }

        public FileBlob File { get; private set; }

        private Material() { }

        public void ChangeFile(FileBlob file)
        {
            this.File = file;
        }

        //public string GetPath()
        //{
        //    return String.Format("article-{0}/{1}{2}", this.Article.ID, this.Code, this.Filename.Extension).ToLower();
        //}

        static public Material Create(DateTimeOffset? date, string title, string comment = "", FileBlob file = null)
        {
            return new Material()
            {
                Date = date,
                Title = title ?? "",
                Comment = comment ?? "",
                File = file
            };
        }
    }
}
