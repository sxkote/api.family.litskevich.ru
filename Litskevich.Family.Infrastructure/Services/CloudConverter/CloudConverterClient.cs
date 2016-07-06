using Litskevich.Family.Infrastructure.Services.CloudConverter.Converters;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Exceptions;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Requests;
using Litskevich.Family.Infrastructure.Services.CloudConverter.Responses;
using Newtonsoft.Json;
using SXCore.Common.Values;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter
{
    public class CloudConverterClient : IDisposable
    {
        public const string ProcessUrl = "https://api.cloudconvert.com/process";

        private string _apiKey;
        private bool _disposed = false;
        private JsonSerializerSettings _jsonSettings;

        public CloudConverterClient(string apiKey)
        {
            _apiKey = apiKey;

            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new LowercaseContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            _jsonSettings.Converters.Add(new DateTimeConverter());
        }

        #region Helper Methods
        private string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
        }

        private T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _jsonSettings);
        }

        private string BuildUrl(string value, string schema = "http")
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invalid (empty) url specified!");

            if (value.StartsWith("//"))
                return $"{schema}:{value}";

            return value;
        }

        private HttpClient BuildHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            return client;
        }

        private async Task<TResponse> SendAsync<TResponse>(string url, object request, HttpMethod httpMethod = null)
            where TResponse : BaseResponse
        {
            TResponse result = null;

            using (var client = this.BuildHttpClient())
            using (var message = new HttpRequestMessage(httpMethod ?? HttpMethod.Post, this.BuildUrl(url)))
            {
                if (request != null)
                    message.Content = new StringContent(this.Serialize(request), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(message))
                {
                    var value = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var error = this.Deserialize<ErrorResponse>(value);
                        throw new ErrorResponseException(error);
                    }

                    result = this.Deserialize<TResponse>(value);
                    result.Code = (int)response.StatusCode;
                }
            }

            return result;
        }

        private async Task<byte[]> DownloadFileAsync(string url)
        {
            byte[] result = null;

            byte[] buffer = new byte[524288];
            using (var client = this.BuildHttpClient())
            using (var ms = new MemoryStream())
            {
                client.Timeout = TimeSpan.FromMinutes(20);
                using (var stream = await client.GetStreamAsync(this.BuildUrl(url)))
                {
                    int readed = 0;
                    while ((readed = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, readed);
                }

                result = ms.ToArray();
            }

            return result;
        }
        #endregion

        #region Functions
        public async Task<ProcessResponse> CreateProcessAsync(string inputFormat, string outputFormat)
        {
            if (String.IsNullOrWhiteSpace(inputFormat))
                throw new ArgumentNullException("inputFormat");

            if (String.IsNullOrWhiteSpace(outputFormat))
                throw new ArgumentNullException("outputFormat");

            var request = new ProcessRequest()
            {
                InputFormat = inputFormat,
                OutputFormat = outputFormat
            };

            return await this.SendAsync<ProcessResponse>(ProcessUrl, request);
        }

        public async Task<StatusResponse> ConvertAsync(InputParameters input, OutputParameters output, ConversionParameters conversion)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            if (conversion == null)
                throw new ArgumentNullException("conversion");

            var process = await this.CreateProcessAsync(input.InputFormat, conversion.OutputFormat);

            var request = new ConvertRequest()
            {
                InputMethod = input.InputMethod,
                Filepath = input.Filepath,
                Filename = input.Filename,
                Tag = input.Tag,

                DownloadMethod = output.DownloadMethod,
                Email = output.Email,
                CallbackUrl = output.CallbackUrl,
                Wait = output.Wait,
                SaveToServer = output.SaveToServer,
                OutputStorage = output.OutputStorage,

                OutputFormat = conversion.OutputFormat,
                ConverterOptions = conversion.ConverterOptions,
                PresetId = conversion.PresetId,
                Timeout = conversion.Timeout,
            };

            return await this.SendAsync<StatusResponse>(process.Url, request);
        }

        public async Task<StatusResponse> GetStatusAsync(string processUrl)
        {
            return await this.SendAsync<StatusResponse>(processUrl, null, HttpMethod.Get);
        }

        public async Task<byte[]> DownloadAsync(string processUrl)
        {
            var status = await this.GetStatusAsync(processUrl);
            if (!status.IsFinished)
                throw new InvalidOperationException("Convertation process is not finished yet!");

            return await this.DownloadFileAsync(status.Output.Url);
        }
        #endregion

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
