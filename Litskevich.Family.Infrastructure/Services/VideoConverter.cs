using System;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Infrastructure.Services.CloudConverter;
using SXCore.Common.Exceptions;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using System.Threading.Tasks;
using System.Threading;
using SXCore.Domain.Entities;

namespace Litskevich.Family.Infrastructure.Services
{
    public class VideoConverter : IVideoConverter
    {
        private IFamilyUnitOfWork _unitOfWork;
        private IFamilyInfrastructureProvider _infrastructure;
        private CloudConverterClient _converter;

        public VideoConverter(IFamilyUnitOfWork unitOfWork, IFamilyInfrastructureProvider infrastructure)
        {
            _unitOfWork = unitOfWork;
            _infrastructure = infrastructure;

            if (_infrastructure == null)
                throw new CustomArgumentException("Invalid Infrastructure provided for VideoConverter!");

            _converter = new CloudConverterClient(_infrastructure.GetSettings("CloudConverterApiKey"));
        }

        public void ConvertVideo(long materialID)
        {
            try
            {
                if (_converter == null)
                    return;

                var material = _unitOfWork.MaterialRepository.Get(materialID);
                if (material == null || material.File == null)
                    return;

                var videoPath = _infrastructure.GetMaterialPath(material);
                var videoUrl = _infrastructure.StorageService.GetFileUrl(videoPath);
                var videoFormat = material.File.FileName.Extension.Trim('.');

                var input = new InputParameters()
                {
                    Filepath = videoUrl,
                    InputFormat = videoFormat.ToLower(),
                    InputMethod = InputMethod.Download
                };

                var output = new OutputParameters()
                {
                    SaveToServer = true
                };

                var conversion = new ConversionParameters()
                {
                    OutputFormat = "mp4"
                };

               var result = _converter.ConvertAsync(input, output, conversion).Result;

                if (result != null && !String.IsNullOrWhiteSpace(result.Url))
                    Task.Run(() => this.WaitForConvertaion(materialID, result.Url));
            }
            catch (Exception ex) { }
        }

        private void WaitForConvertaion(long materialID, string processUrl)
        {
            DateTime start = DateTime.Now;

            if (_converter == null)
                return;

            // waiting for convertation complete
            var status = _converter.GetStatusAsync(processUrl).Result;
            while (!status.IsFinished)
            {
                if ((DateTime.Now - start).Hours >= 1)
                    return;

                Thread.Sleep(5000);
                status = _converter.GetStatusAsync(processUrl).Result;
            }


            this.UpdateConvertedVideoMaterialAsync(materialID, processUrl);
        }

        private async void UpdateConvertedVideoMaterialAsync(long materialID, string processUrl)
        {
            if (_converter == null)
                return;

            // get Material from DataBase
            var material = _unitOfWork.MaterialRepository.Get(materialID);
            if (material == null || material.File == null)
                return;

            // get current path of the current Blob
            var sourcePath = _infrastructure.GetMaterialPath(material);

            // get the status of the convetsion
            var status = await _converter.GetStatusAsync(processUrl);
            if (status == null || !status.IsFinished || status.Output == null)
                return;

            // download conveted file
            var data = await _converter.DownloadAsync(processUrl);
            if (data == null)
                return;

            // change the filename with new extension
            string filename = material.File.FileName.Name.Replace(material.File.FileName.Extension, "." + status.Output.Extension);

            // create new Blob and attach it to the material
            material.ChangeFile(FileBlob.Create(material.Article.Code + "/", filename, "", data.Length));

            // get new path of the new Blob (where to update converted file)
            var convertedPath = _infrastructure.GetMaterialPath(material);

            // update converted file
            _infrastructure.StorageService.SaveFile(convertedPath, data);

            // save changes to DataBase
            _unitOfWork.MaterialRepository.Update(material);
            _unitOfWork.SaveChanges();

            // delete old file from Storage
            _infrastructure.StorageService.DeleteFile(sourcePath);         
        }
    }
}
