using Litskevich.Family.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class ActivityConfiguration : EntityTypeConfiguration<Activity>
    {
        public ActivityConfiguration()
        {
            ToTable("Activity");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("ActivityID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
