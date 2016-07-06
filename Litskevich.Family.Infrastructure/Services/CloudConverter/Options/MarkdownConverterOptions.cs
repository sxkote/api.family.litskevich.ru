using Newtonsoft.Json;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Options
{
    /// <summary>
    /// This represents the converter options entity for markdown.
    /// </summary>
    public partial class MarkdownConverterOptions
    {
        /// <summary>
        /// Gets or sets the input markdown syntax.
        /// </summary>
        [JsonProperty(PropertyName = "input_markdown_syntax", NullValueHandling = NullValueHandling.Include)]
        public string InputMarkdownSyntax { get; set; }

        /// <summary>
        /// Gets or sets the output markdown syntax.
        /// </summary>
        [JsonProperty(PropertyName = "output_markdown_syntax")]
        public string OutputMarkdownSyntax { get; set; }
    }
}