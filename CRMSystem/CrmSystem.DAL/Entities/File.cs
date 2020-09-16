namespace CrmSystem.DAL.Entities
{
    public class File
    {
        public FileType Type { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public string PublicLink { get; set; } = null;
    }
}
