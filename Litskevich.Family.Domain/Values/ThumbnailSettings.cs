namespace Litskevich.Family.Domain.Values
{
    public class ThumbnailSettings
    {
        public const string DefaultSuffix = ".thumbnail.jpg";
        public const string DefaultMimeType = "image/jpeg";
        public const int DefaultSize = 200;
        public const int DefaultQuality = 70;

        static public ThumbnailSettings Default
        {
            get
            {
                return new ThumbnailSettings(DefaultSuffix, DefaultMimeType, DefaultSize, DefaultQuality);
            }
        }

        public string Suffix { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
        public int Quality { get; set; }

        private ThumbnailSettings() { }

        public ThumbnailSettings(string suffix, string mimeType, int size, int quality)
        {
            this.Suffix = suffix;
            this.MimeType = mimeType;
            this.Size = size;
            this.Quality = quality;
        }

    }
}
