using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class AttachedFileType : ObjectGraphType<AttachedFile>
    {
        public AttachedFileType()
        {
            Name = "AttachedFile";

            Field(x => x.AttachedFileId);
            Field(x => x.Name);
            Field(x => x.Path);
            Field<ApplicationUserType>("Owner");
        }
    }
}