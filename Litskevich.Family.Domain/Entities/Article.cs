using SXCore.Common.Entities;
using SXCore.Common.Services;
using SXCore.Common.Values;
using System;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Entities
{
    public class Article : Entity
    {
        public Person Author { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public string Title { get; private set; }
        public string Comment { get; private set; }
        public Period Period { get; private set; }

        public ICollection<Material> Materials { get; private set; }
        public ICollection<Person> Persons { get; private set; }

        private Article()
        {
            this.Materials = new List<Material>();
            this.Persons = new List<Person>();
        }

        public Material AddMaterial(Material material)
        {
            if (material != null)
                this.Materials.Add(material);

            return material;
        }

        public Material AddMaterial(DateTimeOffset? date = null, string title = "", string comment = "", FileBlob file = null)
        {
            var material = Material.Create(date, title, comment, file);

            return this.AddMaterial(material);
        }

        public Person AddPerson(Person person)
        {
            if (person != null)
                this.Persons.Add(person);

            return person;
        }

        static public Article Create(Person author, string title, string comment = "", Period period = null)
        {
            return new Article()
            {
                Author = author,
                Date = CommonService.Now,
                Title = title,
                Comment = comment,
                Period = period
            };
        }
    }
}
