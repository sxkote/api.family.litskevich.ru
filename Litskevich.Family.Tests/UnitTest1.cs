using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Litskevich.Family.Infrastructure.Data;
using Litskevich.Family.Domain.Entities;
using System.Linq;
using Litskevich.Family.Infrastructure.Services.Repositories;

namespace Litskevich.Family.Tests
{
    [TestClass]
    public class FamilyDbContextTests
    {
        public static System.Data.Entity.SqlServer.SqlProviderServices EnsureAssemblySqlServerIsCopied { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            FamilyDbContext context = new FamilyDbContext();
            var persons = context.Set<Person>().ToList();

            var repo = new PersonRepository(context);
            var find = repo.FindAll("лиn");

        }
    }
}
