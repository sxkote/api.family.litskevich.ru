using Litskevich.Family.Domain.Entities;
using SXCore.Common.Entities;
using SXCore.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class PersonConfiguration : EntityTypeConfiguration<Person>
    {
        public PersonConfiguration()
        {
            ToTable("Person");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("PersonID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasOptional<Avatar>(t => t.Avatar)
                .WithMany()
                .Map(m => m.MapKey("AvatarID"))
                .WillCascadeOnDelete(true);

            HasOptional<Manager>(t => t.Manager)
                .WithRequired(m => m.Person)
                //.Map(m => m.MapKey("ManagerID"))
                .WillCascadeOnDelete(true);
        }
    }
}
