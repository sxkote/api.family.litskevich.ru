using Litskevich.Family.Infrastructure.Data.Configurations;
using SXCore.Infrastructure.EF.Data;
using System.Data.Entity;

namespace Litskevich.Family.Infrastructure.Data
{
    public partial class FamilyDbContext : CoreDbContext
    {
        public FamilyDbContext()
            : this("")
        { }

        public FamilyDbContext(string connection)
            : base(string.IsNullOrWhiteSpace(connection) ? "name=FamilyDbConnection": connection)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Complex Types

            // Entities
            modelBuilder.Configurations.Add(new ActivityConfiguration());
            modelBuilder.Configurations.Add(new PersonConfiguration());
            modelBuilder.Configurations.Add(new ManagerConfiguration());
            modelBuilder.Configurations.Add(new ManagerRoleTypeConfiguration());
            modelBuilder.Configurations.Add(new GuestConfiguration());
            modelBuilder.Configurations.Add(new UserTokenConfiguration());
            modelBuilder.Configurations.Add(new ArticleConfiguration());
            modelBuilder.Configurations.Add(new MaterialConfiguration());
        }
    }
}
