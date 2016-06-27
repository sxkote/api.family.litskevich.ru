namespace Litskevich.Family.Domain.Values
{
    public class ImageSettings
    {
        public const bool DefaultResizeLargeImages = true;
        public const int DefaultMaxFileSize = 1572864;
        public const int DefaultMaxWidth = 2000;
        public const int DefaultMaxHeight = 2000;
        public const int DefaultQuality = 90;

        static public ImageSettings Default
        {
            get
            {
                return new ImageSettings(DefaultResizeLargeImages, DefaultMaxFileSize, DefaultMaxWidth, DefaultMaxHeight, DefaultQuality);
            }
        }

        public bool ResizeLargeImages { get; set; }
        public int MaxFileSize { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public int Quality { get; set; }

        private ImageSettings() { }

        public ImageSettings(bool resize, int maxFileSize, int maxWidth, int maxHeight, int quality)
        {
            this.ResizeLargeImages = resize;
            this.MaxFileSize = maxFileSize;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.Quality = quality;
        }
    }
}
