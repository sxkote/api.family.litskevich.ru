using Litskevich.Family.Domain.Contracts;
using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Contracts.Services;
using SXCore.Common.Contracts;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;

namespace Litskevich.Family.Domain.Managers
{
    public class FilesManager : BaseManager, IFilesManager
    {
        public FilesManager(IFamilyUnitOfWork uow, IFamilyInfrastructureProvider infrastructure, ITokenProvider tokenProvider)
            : base(uow, infrastructure, tokenProvider)
        {}

        public FileBlob GetFileBlob(string code)
        {
            return this.UnitOfWork.Find<FileBlob>(code);
        }

        public FileData DownloadAvatar(string code)
        {
            return this.ReadAvatar(code);
        }

        public FileData DownloadFile(string code)
        {
            var blob = this.GetFileBlob(code);
            if (blob == null)
                return null;

            return this.ReadFile(blob.ID);
        }

        public override FileBlob UploadFile(FileUpload upload, string folder = "")
        {
            var blob = base.UploadFile(upload, folder);

            if (blob != null)
                this.SaveChanges();

            return blob;
        }

        public long GetChunksUploadSize(string uploadID)
        {
            var path = this.GetChunksUploadPath(uploadID);
            return this.FileStorageService.SizeOfFile(path);
        }

        //public long GetFileSize(string code)
        //{
        //    var blob = this.GetFileBlob(code);
        //    if (blob == null)
        //        return 0;

        //    return this.FileStorageService.SizeOfFile(blob.GetPath());
        //}

        //public string GetFileUrl(string code, int hours = 24)
        //{
        //    var blob = this.GetFileBlob(code);
        //    if (blob == null)
        //        return "";

        //    return this.FileStorageService.GetFileUrl(blob.GetPath(), hours);
        //}
    }
}
