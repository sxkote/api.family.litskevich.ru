using Litskevich.Family.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class MaterialConfiguration : EntityTypeConfiguration<Material>
    {
        public MaterialConfiguration()
        {
            ToTable("Material");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("MaterialID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Ignore(m => m.FileType);

            HasOptional(m => m.File)
                .WithMany()
                .Map(m => m.MapKey("FileBlobID"));
        }
    }
}
