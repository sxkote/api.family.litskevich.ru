﻿namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Requests
{
    /// <summary>
    /// This represents the process request entity.
    /// </summary>
    public partial class ProcessRequest : BaseRequest
    {
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the input format.
        /// </summary>
        public string InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the output format.
        /// </summary>
        public string OutputFormat { get; set; }
    }
}
