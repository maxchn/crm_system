using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class FileTypeType : EnumerationGraphType<DAL.Entities.FileType>
    {
        public FileTypeType()
        {
            Name = "FileType";
        }
    }
}