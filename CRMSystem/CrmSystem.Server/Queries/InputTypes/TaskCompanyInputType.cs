using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskCompanyInputType : InputObjectGraphType
    {
        public TaskCompanyInputType()
        {
            Name = "TaskCompanyInput";

            Field<NonNullGraphType<IntGraphType>>("CompanyId");
        }
    }
}