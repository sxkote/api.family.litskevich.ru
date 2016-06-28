using Litskevich.Family.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class GuestConfiguration : EntityTypeConfiguration<Guest>
    {
        public GuestConfiguration()
        {
            ToTable("Guest");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("GuestID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
