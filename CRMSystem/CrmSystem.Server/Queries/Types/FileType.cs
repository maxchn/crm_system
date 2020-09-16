using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class FileType : ObjectGraphType<File>
    {
        public FileType()
        {
            Name = "File";

            Field<FileTypeType>("FileType");
            Field(x => x.Name);
            Field(x => x.Path);
        }
    }
}
