using Litskevich.Family.Domain.Events;
using SXCore.Common.Classes;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litskevich.Family.Domain.Entities
{
    public class Article : Entity
    {
        public const int DefaultPageSize = 20;

        public string Code { get; private set; }

        public Person Author { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public string Title { get; private set; }
        public DateTime? PeriodBegin { get; private set; }
        public DateTime? PeriodEnd { get; private set; }
        public string Comment { get; private set; }

        public ICollection<Material> Materials { get; private set; }
        public ICollection<Person> Members { get; private set; }

        private Article()
        {
            this.Materials = new List<Material>();
            this.Members = new List<Person>();
        }

        public Material AddMaterial(FileBlob file)
        {
            if (this.ID <= 0)
                throw new CustomOperationException("Can't add material to unexisting article");

            var material = Material.Create(this, file);

            this.Materials.Add(material);

            DomainEvents.Raise(new MaterialAddedToArticleEvent(this, material));

            return material;
        }

        public Material RemoveMaterial(long materialID)
        {
            var material = this.Materials.FirstOrDefault(m => m.ID == materialID);
            if (material == null)
                return null;

            this.Materials.Remove(material);

            DomainEvents.Raise(new MaterialDeletedEvent(this, material));

            return material;
        }

        //public Person AddMember(Person person)
        //{
        //    if (person == null)
        //        return null;

        //    if (this.Members.Any(m => m.ID == person.ID))
        //        return null;

        //    this.Members.Add(person);

        //    DomainEvents.Raise(new MembersChangedEvent(this, person));

        //    return person;
        //}

        public void ChangeMembers(IEnumerable<Person> members)
        {
            if (members == null)
                return;

            // delete Members (Persons), that are not exist in new Member collection
            var memberToDelete = this.Members.Where(p => members.All(m => m.ID != p.ID)).ToList();
            memberToDelete.ForEach(p => this.Members.Remove(p));

            // new members to add
            var membersToAdd = members.Where(m => this.Members.All(p => p.ID != m.ID)).ToList();
            membersToAdd.ForEach(m => this.Members.Add(m));

            DomainEvents.Raise(new MembersChangedEvent(this, this.Members));
        }

        public void Change(string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "")
        {
            this.Title = title ?? "";
            this.PeriodBegin = periodBegin;
            this.PeriodEnd = periodEnd;
            this.Comment = comment ?? "";
        }

        static public Article Create(DateTimeOffset date, Person author, string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "")
        {
            var article = new Article()
            {
                Code = "a" + CommonService.GenerateCode(8, capitalize: false),
                Author = author,
                Date = date
            };

            article.Change(title, periodBegin, periodEnd, comment);

            return article;
        }
    }
}
