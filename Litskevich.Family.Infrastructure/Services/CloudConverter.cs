using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Litskevich.Family.Infrastructure.Services
{
    public class CloudConverterClient : IDisposable
    {
        private string _apikey;
        private HttpClient _client;

        public CloudConverterClient(string apikey)
        {
            _apikey = apikey;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apikey);
        }

        private string Serialize<T>(T obj)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new LowercaseContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }

        private T DeSerialize<T>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        private TResponse Send<TResponse, TRequest>(string url, TRequest request, string method = "POST")
        {
            try
            {
                var data = this.Serialize(request);

                var response = _client.UploadString(url.StartsWith("//") ? "http:" + url : url, method, data);

                return this.DeSerialize<TResponse>(response);
            }
            catch (Exception ex) { return default(TResponse); }
        }

        private TResponse Send<TResponse>(string url, string method = "GET")
        {
            try
            {
                var response = _client.DownloadString(url.StartsWith("//") ? "http:" + url : url);

                return this.DeSerialize<TResponse>(response);
            }
            catch { return default(TResponse); }
        }

        public Status Convert(string inputFormat, string outputFormat, string fileurl, ConverterOptions convertOptions = null)
        {
            try
            {
                var request = new ConvertRequest(inputFormat, outputFormat, fileurl, convertOptions);

                var response = this.Send<Status, ConvertRequest>("https://api.cloudconvert.com/convert", request);

                return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public StartProccessResponse StartProccess(string processUrl, StartProccessRequest request)
        {
            try
            {
                var response = this.Send<StartProccessResponse, StartProccessRequest>(processUrl, request);

                return response;
            }
            catch { return null; }
        }

        public Status GetStatus(string processUrl)
        {
            try
            {
                return this.Send<Status>(processUrl);
            }
            catch { return null; }
        }

        ///// <summary>
        ///// Cancels a running conversion
        ///// </summary>
        ///// <param name="processUrl">The URL to upload a file which you get using the GetProcessURL method.</param>
        ///// <returns></returns>
        //public string CancelConversion(string processUrl)
        //{
        //    try
        //    {
        //        using (WebClient client = new WebClient())
        //        {
        //            client.Headers["Content-Type"] = "application/www-x-form-urlencoded";

        //            return client.DownloadString(string.Format("{0}/cancel", processUrl));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        ///// <summary>
        ///// Deletes a finished or an aborted conversion (This is irreversible).
        ///// </summary>
        ///// <param name="processUrl">The URL to upload a file which you get using the GetProcessURL method.</param>
        ///// <returns></returns>
        //public string DeleteConversion(string processUrl)
        //{
        //    try
        //    {
        //        using (WebClient client = new WebClient())
        //        {
        //            client.Headers["Content-Type"] = "application/www-x-form-urlencoded";

        //            return client.DownloadString(string.Format("{0}/delete", processUrl));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        ///// <summary>
        ///// Lists all the running conversions.
        ///// </summary>
        ///// <returns></returns>
        //public string ListProcesses()
        //{
        //    try
        //    {
        //        using (WebClient client = new WebClient())
        //        {
        //            client.Headers["Content-Type"] = "application/www-x-form-urlencoded";

        //            return client.DownloadString(string.Format("https://api.cloudconvert.org/processes?apikey={0}", _apikey));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }
    }


    
    public class ConvertRequest
    {
        public string inputformat { get; set; }
        public string outputformat { get; set; }
        public string input { get; set; } = "download";
        public string file { get; set; }
        public bool wait { get; set; } = false;
        public bool download { get; set; } = false;
        public ConverterOptions converteroptions { get; set; }

        public ConvertRequest() { }

        public ConvertRequest(string inputformat, string outputformat, string fileurl, ConverterOptions convertOptions = null)
        {
            this.inputformat = inputformat;
            this.outputformat = outputformat;
            this.file = fileurl;
            this.converteroptions = convertOptions;
        }
    }

    public class CreateProcessRequest
    {
        public const string DefaultMode = "convert";

        public string inputformat { get; set; }
        public string outputformat { get; set; }
        public string mode { get; set; }

        public CreateProcessRequest() { }

        public CreateProcessRequest(string inputformat, string outputformat, string mode = DefaultMode)
        {
            this.inputformat = inputformat;
            this.outputformat = outputformat;
            this.mode = mode;
        }
    }

    public class CreateProcessResponse
    {
        public string url { get; set; }
        public string id { get; set; }
        public string host { get; set; }
        public string expires { get; set; }
        public string maxtime { get; set; }
        public string minutes { get; set; }

        public CreateProcessResponse() { }
    }

    public class StartProccessRequest
    {
        // Input Parameters
        public string input { get; set; }
        public string file { get; set; }
        public string[] files { get; set; }
        public string filename { get; set; }
        public string tag { get; set; }

        // Conversion Parameters
        public string outputformat { get; set; }
        public ConverterOptions converteroptions { get; set; }
        public string preset { get; set; }
        public string mode { get; set; }
        public int timeout { get; set; }

        // Output Parameters
        public string email { get; set; }
        public string output { get; set; }
        public string callback { get; set; }
        public bool wait { get; set; }
        public bool download { get; set; }
        public bool save { get; set; }

        public StartProccessRequest() { }

        static public StartProccessRequest CreateForVideoInfo(string fileurl, string filename)
        {
            return new StartProccessRequest()
            {
                mode = "info",
                input = "download",
                file = fileurl,
                filename = filename,
                outputformat = "mp4",
                converteroptions = new ConverterOptions
                {
                    video_codec = "H264",
                },
                download = true,
                save = true
            };
        }

        static public StartProccessRequest CreateForVideo(string fileurl)
        {
            return new StartProccessRequest()
            {
                input = "download",
                file = fileurl,
                outputformat = "mp4",
                converteroptions = new ConverterOptions
                {
                    video_resolution = "1280x720",
                    video_codec = "H264",
                    video_crf = 23,
                    faststart = true
                },
                timeout = 0,
                download = false,
                save = false
            };
        }
    }

    public class StartProccessResponse
    {
        public string id { get; set; }
        public string url { get; set; }
        public DateTime expire { get; set; }
        public int percent { get; set; }
        public string message { get; set; }
        public string step { get; set; }
        public DateTime starttime { get; set; }

        public StartProccessResponse() { }
    }

    public class Status
    {
        public string id { get; set; }
        public string url { get; set; }
        public double percent { get; set; }
        public string message { get; set; }
        public string step { get; set; }
        public string path { get; set; }
        public double minutes { get; set; }
        public DateTime starttime { get; set; }
        public DateTime expire { get; set; }
        public DateTime endtime { get; set; }
        public Input input { get; set; }
        public Converter converter { get; set; }
        public Output output { get; set; }
    }

    public class Input
    {
        public string url { get; set; }
        public string type { get; set; }
        public string filename { get; set; }
        public long size { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
    }

    public class Converter
    {
        public string mode { get; set; }
        public string format { get; set; }
        public string type { get; set; }
        public Options options { get; set; }
        public double duration { get; set; }
    }

    public class Options
    {
        public string inputformat { get; set; }
        public string outputformat { get; set; }
        public string converter { get; set; }
        public ConverterOptions converteroptions { get; set; }
        public string note { get; set; }
        public string group { get; set; }

    }

    public class ConverterOptions
    {
        public string video_codec { get; set; }
        public string video_bitrate { get; set; }
        public string video_resolution { get; set; }
        public string video_ratio { get; set; }
        public string video_fps { get; set; }
        public int video_crf { get; set; }
        public string video_qscale { get; set; }

        public string audio_codec { get; set; }
        public string audio_bitrate { get; set; }
        public string audio_frequency { get; set; }
        public string audio_normalize { get; set; }
        public string audio_qscale { get; set; }

        public string trim_from { get; set; }
        public string trim_to { get; set; }

        public string outputprofile { get; set; }
        public string authors { get; set; }
        public string title { get; set; }

        public string resize { get; set; }
        public string resizemode { get; set; }
        public string rotate { get; set; }

        public string quality { get; set; }
        public string density { get; set; }

        public string page_range { get; set; }
        public string password { get; set; }

        public string autocad_version { get; set; }

        public string no_images { get; set; }

        public string mergelayers { get; set; }

        public bool faststart { get; set; }

    }

    public class Output
    {
        public string filename { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public int downloads { get; set; }
    }

    /// <summary>
    /// The class for deserializing the list of running processes.
    /// </summary>
    public class ListRC
    {
        public string id { get; set; }
        public string host { get; set; }
        public string step { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string url { get; set; }
    }
}
