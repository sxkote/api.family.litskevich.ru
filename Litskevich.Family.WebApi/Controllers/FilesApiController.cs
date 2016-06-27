using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.WebApi.Models;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.Common;
using SXCore.Common.Contracts;
using SXCore.Common.Entities;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using SXCore.WebApi;
using SXCore.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [ApiAuthorize]
    [ApiException]
    [RoutePrefix("api/file")]
    public class FilesApiController : CoreApiController
    {
        public const string IconPath = "~/Content/Images/content-types/";

        private IFilesManager _manager;

        public IFilesManager Manager
        { get { return _manager; } }

        public FilesApiController(IFilesManager manager, ICacheProvider cacheProvider)
            : base()
        {
            _manager = manager;

            if (this.CacheProvider == null)
                this.CacheProvider = cacheProvider;
        }

        [ApiAuthorize(Roles = "Supervisor,Admin")]
        [HttpPost]
        [Route("")]
        public async Task<FileBlobModel> Post()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Temp storage location for File Chunks
            MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();

            // Read all contents of multipart message into MultipartMemoryStreamProvider.                 
            await Request.Content.ReadAsMultipartAsync(provider);

            var content = provider.Contents[0];
            if (content == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var upload = await GetFileUpload(content);
            if (upload == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var blob = _manager.UploadFile(upload);
            if (blob != null)
                return blob.ToSerial();

            return new FileBlobModel()
            {
                //FileID = 0,
                Code = upload.UploadID,
                FileName = upload.FileName,
                Size = _manager.GetChunksUploadSize(upload.UploadID)
            };
            //return Request.CreateResponse(HttpStatusCode.OK);
        }

        [ApiAuthorize(Roles = "Supervisor,Admin")]
        [HttpPost]
        [Route("flow")]
        public async Task<FileBlobModel> PostFlow()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Temp storage location for File Chunks
            MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();

            // Read all contents of multipart message into MultipartMemoryStreamProvider.                 
            await Request.Content.ReadAsMultipartAsync(provider);

            var chunkContent = provider.Contents.SingleOrDefault(c => c.Headers.ContentDisposition.Name.Trim('"', ' ').Equals("file", StringComparison.OrdinalIgnoreCase));
            if (chunkContent == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            byte[] data = null;
            using (Stream chunkStream = await chunkContent.ReadAsStreamAsync())
                data = chunkStream.ReadFully();

            Func<string, string> getParamFromBody = name => provider.Contents
                .Single(c => c.Headers.ContentDisposition.Name.Trim('"', ' ').Equals(name, StringComparison.OrdinalIgnoreCase))
                .ReadAsStringAsync().Result;

            var chunkNumber = getParamFromBody("flowChunkNumber");
            var totalSize = getParamFromBody("flowTotalSize");
            var uploadID = getParamFromBody("flowUploadID");
            var fileName = getParamFromBody("flowFileName");

            var upload = new FileUpload(uploadID, fileName, data, Convert.ToInt64(totalSize), Convert.ToInt32(chunkNumber));

            var blob = _manager.UploadFile(upload);
            if (blob != null)
                return blob.ToSerial();

            return new FileBlobModel()
            {
                //FileID = 0,
                Code = upload.UploadID,
                FileName = upload.FileName,
                Size = _manager.GetChunksUploadSize(upload.UploadID)
            };
            //return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("{code}")]
        public FileBlobModel GetFileBlob(string code)
        {
            return _manager.GetFileBlob(code).ToSerial();
        }

        //[HttpGet]
        //[Route("{code}/url")]
        //public string GetFileUrl(string code)
        //{
        //    return _manager.GetFileUrl(code);
        //}

        [HttpGet]
        [Route("{code}/content")]
        public HttpResponseMessage GetFileContent(string code)
        {
            return this.ReturnFile(_manager.DownloadFile(code));
        }

        [HttpGet]
        [Route("{code}/avatar")]
        public HttpResponseMessage GetFileAvatar(string code)
        {
            var cacheKey = $"icon-{code}";

            var content = this.CacheProvider?.Get<FileData>(cacheKey);

            if (content == null)
            {
                content = this.ComputeFileAvatar(code);

                if (this.CacheProvider != null && content != null)
                    this.CacheProvider.Set(cacheKey, content);
            }

            return this.ReturnFile(content);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("~/api/avatar/{code}")]
        public HttpResponseMessage GetAvatar(string code)
        {
            var cacheKey = $"avatar-{code}";

            var content = this.CacheProvider?.Get<FileData>(cacheKey);

            if (content == null)
            {
                content = _manager.DownloadAvatar(code);
                if (this.CacheProvider != null)
                    this.CacheProvider.Set(cacheKey, content);
            }

            if (content == null)
                content = IconFileData(null);

            return this.ReturnFile(content);
        }

        protected FileData ComputeFileAvatar(string code, int maxSize = 200)
        {
            var blob = _manager.GetFileBlob(code);
            if (blob == null)
                return IconFileData(null);

            if (blob.FileName.Type != FileName.FileType.Image)
                return IconFileData(blob.FileName);

            FileData data = _manager.DownloadFile(blob.Code);
            if (data == null)
                return IconFileData(blob.FileName);

            return Avatar.MakeAvatarFile(data.Data, maxSize);
        }

        #region Static Functions
        public static async Task<FileUpload> GetFileUpload(HttpContent content)
        {
            if (content == null || content.Headers == null)
                return null;

            Func<string, string> getHeaderValue = name => content.Headers.FirstOrDefault(h => h.Key.Equals(name, CommonService.StringComparison)).Value?.FirstOrDefault();

            try
            {
                FileUpload upload = null;

                using (Stream chunkStream = await content.ReadAsStreamAsync())
                {
                    //Check for not null or empty
                    if (chunkStream == null)
                        throw new HttpResponseException(HttpStatusCode.NotFound);

                    var filename = content.Headers.ContentDisposition.FileName.Trim('"', ' ');

                    // Read file chunk detail
                    var headerUploadID = getHeaderValue("Upload-UploadID");
                    var headerChunkID = getHeaderValue("Upload-ChunkID");
                    var headerTotalSize = getHeaderValue("Upload-TotalSize");

                    var data = chunkStream.ReadFully();

                    int chunkID = String.IsNullOrWhiteSpace(headerChunkID) ? -1 : Convert.ToInt32(headerChunkID);
                    long totalSize = String.IsNullOrWhiteSpace(headerTotalSize) ? 0 : Convert.ToInt64(headerTotalSize);

                    upload = new FileUpload(headerUploadID, filename, data, totalSize, chunkID);
                }

                return upload;
            }
            catch { return null; }
        }

        public static FileData IconFileData(FileName filename)
        {
            var data = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath(IconPath + IconFileName(filename)));

            var name = filename == null ? new FileName("unknown.png") : filename;

            return new FileData(name, data);
        }

        public static string IconFileName(FileName filename)
        {
            if (filename == null)
                return "Unknown.png";

            switch (filename.Type)
            {
                case FileName.FileType.Video:
                case FileName.FileType.Audio:
                    return "Multimedia.png";

                case FileName.FileType.Excel:
                    return "Excel.png";

                case FileName.FileType.Word:
                    return "Word.png";

                case FileName.FileType.PowerPoint:
                    return "PowerPoint.png";

                case FileName.FileType.Text:
                    return "Text.png";

                case FileName.FileType.Image:
                    return "Image.png";

                case FileName.FileType.PDF:
                    return "PDF.png";

                default: return "File.png";
            }
        }
        #endregion
    }
}
