using CrmSystem.Server.GraphQL;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class EmployeeRegisterResultType : ObjectGraphType<EmployeeRegisterResult>
    {
        public EmployeeRegisterResultType()
        {
            Name = "EmployeeRegisterResult";

            Field(x => x.Status);
            Field<ApplicationUserType>("Employee");
        }
    }
}