using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskAttachedFileType : ObjectGraphType<TaskAttachedFile>
    {
        public TaskAttachedFileType()
        {
            Name = "TaskAttachedFile";

            Field(x => x.TaskAttachedFileId);
            Field(x => x.AttachedFileId);
            Field(x => x.TaskId);
            Field<AttachedFileType>("AttachedFile");
        }
    }
}