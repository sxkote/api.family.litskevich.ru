using Litskevich.Family.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class ManagerTokenConfiguration : EntityTypeConfiguration<ManagerToken>
    {
        public ManagerTokenConfiguration()
        {
            ToTable("ManagerToken");
            HasKey(t => t.ID);

            Property(t => t.ID)
                .HasColumnName("ManagerTokenID")
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            //link: Person
            HasRequired<Manager>(t => t.Manager)
                .WithMany()
                .Map(m => m.MapKey("ManagerID"));
        }
    }

}
