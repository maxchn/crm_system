using System.Collections.Generic;

namespace CrmSystem.Server.Models
{
    public class DeleteFilesModel
    {
        public List<string> Paths { get; set; }

        public int CompanyId { get; set; }
    }
}