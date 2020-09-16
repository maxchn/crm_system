using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskTagInputType : InputObjectGraphType
    {
        public TaskTagInputType()
        {
            Name = "TaskTagInput";

            Field<IntGraphType>("TaskId");
            Field<IntGraphType>("TagId");
            Field<NonNullGraphType<TagInputType>>("Tag");
        }
    }
}