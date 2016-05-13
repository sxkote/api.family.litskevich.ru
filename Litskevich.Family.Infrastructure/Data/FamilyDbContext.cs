using Litskevich.Family.Infrastructure.Data.Configurations;
using SXCore.Infrastructure.EF.Data;
using System.Data.Entity;

namespace Litskevich.Family.Infrastructure.Data
{
    public partial class FamilyDbContext : CoreDbContext
    {
        public FamilyDbContext()
            : base("name=FamilyDbConnection")
        { }

        public FamilyDbContext(string connection)
            : base(connection)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Complex Types
         
            // Entities
            modelBuilder.Configurations.Add(new PersonConfiguration());
            modelBuilder.Configurations.Add(new ManagerConfiguration());
            modelBuilder.Configurations.Add(new ManagerRoleTypeConfiguration());
            modelBuilder.Configurations.Add(new ArticleConfiguration());
            modelBuilder.Configurations.Add(new MaterialConfiguration());
        }
    }
}
