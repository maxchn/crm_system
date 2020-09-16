using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskTagType : ObjectGraphType<TaskTag>
    {
        public TaskTagType()
        {
            Name = "";

            Field(x => x.Id);
            Field(x => x.TaskId);

            Field(x => x.TagId);
            Field<TagType>("Tag");
        }
    }
}