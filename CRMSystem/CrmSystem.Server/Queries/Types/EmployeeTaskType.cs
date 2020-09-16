using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class EmployeeTaskType : ObjectGraphType<DAL.Entities.Observer>
    {
        public EmployeeTaskType()
        {
            Name = "EmployeeTask";

            Field(x => x.TaskId);
            Field(x => x.Id);
            Field<ApplicationUserType>("User");
        }
    }
}