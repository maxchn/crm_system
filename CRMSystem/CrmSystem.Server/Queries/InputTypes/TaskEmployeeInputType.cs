using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskEmployeeInputType : InputObjectGraphType
    {
        public TaskEmployeeInputType()
        {
            Name = "TaskEmployeeInput";

            Field<IntGraphType>("TaskId");
            Field<NonNullGraphType<StringGraphType>>("Id");
        }
    }
}