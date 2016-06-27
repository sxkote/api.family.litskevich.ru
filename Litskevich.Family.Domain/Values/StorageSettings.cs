namespace Litskevich.Family.Domain.Values
{
    public class StorageSettings
    {
        public const string DefaultFolderEmpty = "empty/";
        public const string DefaultFolderDeleted = "deleted/";
        public const string DefaultSASTokenName = "sas";
        public const string DefaultUrl = "";


        static public StorageSettings Default
        {
            get
            {
                return new StorageSettings(DefaultFolderEmpty, DefaultFolderDeleted, DefaultSASTokenName, DefaultUrl);
            }
        }

        public string FolderEmpty { get; set; }
        public string FolderDeleted { get; set; }
        public string SASTokenName { get; set; }
        public string Url { get; set; }

        private StorageSettings() { }

        public StorageSettings(string folderEmpty, string folderDeleted, string sasTokenName, string url)
        {
            this.FolderEmpty = folderEmpty;
            this.FolderDeleted = folderDeleted;
            this.SASTokenName = sasTokenName;
            this.Url = url;
        }
    }
}
