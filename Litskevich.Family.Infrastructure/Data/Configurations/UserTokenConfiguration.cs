using Litskevich.Family.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Litskevich.Family.Infrastructure.Data.Configurations
{
    public class UserTokenConfiguration : EntityTypeConfiguration<UserToken>
    {
        public UserTokenConfiguration()
        {
            ToTable("UserToken");
            HasKey(t => t.ID);

            Property(t => t.ID)
                .HasColumnName("UserTokenID")
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            HasOptional<Manager>(t => t.Manager)
                .WithMany()
                .Map(m => m.MapKey("ManagerID"));

            HasOptional<Guest>(t => t.Guest)
                .WithMany()
                .Map(m => m.MapKey("GuestID"));
        }
    }

}
