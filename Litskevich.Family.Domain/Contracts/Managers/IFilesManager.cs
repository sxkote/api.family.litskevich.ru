using SXCore.Common.Values;
using SXCore.Domain.Contracts;
using SXCore.Domain.Entities;

namespace Litskevich.Family.Domain.Contracts.Managers
{
    public interface IFilesManager : ICoreManager
    {
        FileBlob GetFileBlob(string code);

        FileData DownloadAvatar(string code);
        FileData DownloadFile(string code);

        FileBlob UploadFile(FileUpload upload, string folder = "");

        long GetChunksUploadSize(string uploadID);
    }
}
