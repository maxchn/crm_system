using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskAttachedFileInputType : InputObjectGraphType
    {
        public TaskAttachedFileInputType()
        {
            Name = "TaskAttachedFileInput";

            Field<IntGraphType>("TaskAttachedFileId");
            Field<IntGraphType>("AttachedFileId");
            Field<AttachedFileInputType>("AttachedFile");
            Field<IntGraphType>("TaskId");
        }
    }
}