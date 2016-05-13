using Litskevich.Family.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class ManagerConfiguration : EntityTypeConfiguration<Manager>
    {
        public ManagerConfiguration()
        {
            ToTable("Manager");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("ManagerID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasMany(m => m.Roles)
               .WithMany()
               .Map(x =>
               {
                   x.MapLeftKey("ManagerID");
                   x.MapRightKey("ManagerRoleTypeID");
                   x.ToTable("ManagerRoles");
               });
        }
    }

    public class ManagerRoleTypeConfiguration : EntityTypeConfiguration<ManagerRoleType>
    {
        public ManagerRoleTypeConfiguration()
        {
            ToTable("TypesManagerRole");
            HasKey(t => t.ID);

            Property(t => t.ID)
                .HasColumnName("ManagerRoleTypeID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
