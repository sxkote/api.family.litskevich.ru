using Litskevich.Family.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class ArticleConfiguration : EntityTypeConfiguration<Article>
    {
        public ArticleConfiguration()
        {
            ToTable("Article");
            HasKey(t => t.ID);

            Property(p => p.ID)
                .HasColumnName("ArticleID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(p => p.Period.Begin)
                .HasColumnName("PeriodBegin");
            Property(p => p.Period.End)
                .HasColumnName("PeriodEnd");

            HasRequired(a => a.Author)
                .WithMany()
                .Map(m => m.MapKey("AuthorID"));

            HasMany(a => a.Persons)
                .WithMany()
                .Map(ap =>
                {
                    ap.MapLeftKey("ArticleID");
                    ap.MapRightKey("PersonID");
                    ap.ToTable("ArticlePersons");
                });

            HasMany(a => a.Materials)
                .WithRequired()
                .Map(m => m.MapKey("ArticleID"))
                .WillCascadeOnDelete(true);
        }
    }
}
