using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskInputType : InputObjectGraphType
    {
        public TaskInputType()
        {
            Name = "TaskInput";

            Field<IntGraphType>("TaskId");
            Field<NonNullGraphType<StringGraphType>>("Name");
            Field<NonNullGraphType<StringGraphType>>("Body");
            Field<NonNullGraphType<DateTimeGraphType>>("Deadline");
            Field<NonNullGraphType<AuthorInputType>>("Author");
            Field<NonNullGraphType<BooleanGraphType>>("IsImportant");
            Field<NonNullGraphType<TaskCompanyInputType>>("Company");
            Field<NonNullGraphType<ListGraphType<TaskEmployeeInputType>>>("CoExecutors");
            Field<NonNullGraphType<ListGraphType<TaskEmployeeInputType>>>("Observers");
            Field<NonNullGraphType<ListGraphType<TaskEmployeeInputType>>>("ResponsiblesForExecution");
            Field<NonNullGraphType<ListGraphType<TaskTagInputType>>>("TaskTags");
            Field<ListGraphType<TaskAttachedFileInputType>>("AttachedFiles");
        }
    }
}