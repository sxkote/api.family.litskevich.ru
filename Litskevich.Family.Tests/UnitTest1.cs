using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Litskevich.Family.Infrastructure.Data;
using Litskevich.Family.Domain.Entities;
using System.Linq;
using Litskevich.Family.Infrastructure.Services.Repositories;
using Litskevich.Family.Domain.Managers;
using Litskevich.Family.Infrastructure.Services;
using SXCore.Common.Services;
using System.IO;
using System.Diagnostics;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Infrastructure.Services.CloudConverter;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Responses;
using SXCore.Common.Values;
using System.Threading;
using System.Threading.Tasks;

namespace Litskevich.Family.Tests
{
    [TestClass]
    public class FamilyDbContextTests
    {
        public const string ImageFileName = "foto.jpg";

        public static System.Data.Entity.SqlServer.SqlProviderServices EnsureAssemblySqlServerIsCopied { get; set; }

        string _cloudConverterApiKey = "";
        string _onlineVideoUrl = "";

        private FamilyUnitOfWork _uow;
        private IFamilyInfrastructureProvider _infrastructure = new FamilyInfrastructureProvider();

        [TestInitialize]
        public void Init()
        {
            _uow = new FamilyUnitOfWork();
            _cloudConverterApiKey = _infrastructure.GetSettings("CloudConverterApiKey");
            _onlineVideoUrl = _infrastructure.GetSettings("OnlineVideoUrl");
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
        public void test_video_convert()
        {
            CloudConverterClient converter = new CloudConverterClient(_cloudConverterApiKey);
            var process = converter.ConvertAsync(InputParameters.Create("mov", _onlineVideoUrl), OutputParameters.Create(), ConversionParameters.Create("mp4")).Result;

            var status = converter.GetStatusAsync(process.Url).Result;
            while (!status.IsFinished)
            {
                Thread.Sleep(3000);
                status = converter.GetStatusAsync(process.Url).Result;
            }

            var bytes = converter.DownloadAsync(process.Url).Result;
            File.WriteAllBytes(status.Output.Filename, bytes);
        }
    }
}
