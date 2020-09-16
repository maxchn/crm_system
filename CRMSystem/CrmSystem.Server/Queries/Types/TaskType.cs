using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskType : ObjectGraphType<MTask>
    {
        public TaskType()
        {
            Name = "Task";

            Field(x => x.TaskId);
            Field(x => x.Name);
            Field(x => x.Body);
            Field(x => x.CreatedDate);
            Field(x => x.Deadline);
            Field<ApplicationUserType>("author");
            Field(x => x.IsExecution);
            Field(x => x.StartExecution);
            Field(x => x.EndExecution);
            Field(x => x.FinalPerformerId);
            Field<ApplicationUserType>("finalPerformer");
            Field(x => x.TotalTime);
            Field(x => x.IsImportant);
            Field<CompanyType>("company");
            Field<ListGraphType<EmployeeTaskType>>("CoExecutors");
            Field<ListGraphType<EmployeeTaskType>>("Observers");
            Field<ListGraphType<EmployeeTaskType>>("ResponsiblesForExecution");
            Field<ListGraphType<TaskTagType>>("TaskTags");
            Field<ListGraphType<TaskAttachedFileType>>("AttachedFiles");
        }
    }
}