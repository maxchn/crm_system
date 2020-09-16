using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskCommentInputType : InputObjectGraphType
    {
        public TaskCommentInputType()
        {
            Name = "TaskCommentInput";

            Field<NonNullGraphType<StringGraphType>>("Text");
            Field<NonNullGraphType<IntGraphType>>("TaskId");
            Field<NonNullGraphType<StringGraphType>>("UserId");
            Field<NonNullGraphType<DateTimeGraphType>>("DateTime");
        }
    }
}