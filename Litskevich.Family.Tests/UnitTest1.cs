using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Litskevich.Family.Infrastructure.Data;
using Litskevich.Family.Domain.Entities;
using System.Linq;
using Litskevich.Family.Infrastructure.Services.Repositories;
using Litskevich.Family.Domain.Managers;
using Litskevich.Family.Infrastructure.Services;
using SXCore.Common.Contracts;
using SXCore.Infrastructure.Services;
using SXCore.Common.Services;
using System.IO;
using System.Diagnostics;
using SXCore.Infrastructure.Services.FileStorage;
using Microsoft.WindowsAzure.Storage;
using Litskevich.Family.Domain.Contracts.Services;

namespace Litskevich.Family.Tests
{
    [TestClass]
    public class FamilyDbContextTests
    {
        public const string ImageFileName = "foto.jpg";

        public static System.Data.Entity.SqlServer.SqlProviderServices EnsureAssemblySqlServerIsCopied { get; set; }

        private FamilyUnitOfWork _uow;
        private IFamilyInfrastructureProvider _infrastructure = new FamilyInfrastructureProvider();

        public void Init()
        {
            _uow = new FamilyUnitOfWork();
        }

        [TestMethod]
        public void test_get_persons()
        {
            FamilyDbContext context = new FamilyDbContext();
            var persons = context.Set<Person>().ToList();

            var repo = new PersonRepository(context);
            File.WriteAllText("hello.txt", "hello");
            var find = repo.FindAll("ли");
            Assert.IsTrue(find.Count() > 0);
        }

        [TestMethod]
        public void create_manager_for_first_user()
        {
            this.Init();

            PersonsManager manager = new PersonsManager(_uow, _infrastructure, null);
            manager.CreateManager(1, "lia", "lia", "User,Supervisor,Admin");

        }

        [TestMethod]
        [DeploymentItem("foto.jpg")]
        public void test_imager_props_foto()
        {
            var path = ImageFileName;
            var path2 = "3.jpg";
            var path3 = "4.jpg";
            Imager img = Imager.Create(File.ReadAllBytes(path));


            Debug.WriteLine(img.Info.Title);
            Debug.WriteLine(img.Info.Comment);
            Debug.WriteLine(img.Info.Date == null ? "null" : img.Info.Date.ToString());

            img.Info.Title = "говое название ";
            img.Info.Comment = "КОИИИИИж'";
            img.Info.Date = new DateTime(2016, 6, 23, 17, 19, 0);

            File.WriteAllBytes(path, img.Save());

            Debug.WriteLine(img.Info.Title);
            Debug.WriteLine(img.Info.Comment);
            Debug.WriteLine(img.Info.Date == null ? "null" : img.Info.Date.ToString());

            File.WriteAllBytes(path2, img.Save());

            File.WriteAllBytes(path3, img.Resize(200).Save(90));
        }

        [TestMethod]
        public void test_email()
        {
            //FamilyInfrastructureProvider i = new FamilyInfrastructureProvider();
            //i.EmailService.SendNotification("soth@list.ru", new SXCore.Common.Values.Message("info lit", "hello message"));
        }
    }
}
