using System.Collections.Generic;

namespace CrmSystem.Server.Models
{
    public class DownloadFilesModel
    {
        public List<string> FilesPaths { get; set; }
        public int CompanyId { get; set; }
    }
}
