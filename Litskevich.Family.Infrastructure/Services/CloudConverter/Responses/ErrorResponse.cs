namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Responses
{
    /// <summary>
    /// This represents the error response entity.
    /// </summary>
    public class ErrorResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Error { get; set; }
    }
}
