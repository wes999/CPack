namespace CPack
{
    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string IncludePath { get; set; }
        public string Description { get; set; }

        public List<string> LibraryFiles { get; set; }
    }
}