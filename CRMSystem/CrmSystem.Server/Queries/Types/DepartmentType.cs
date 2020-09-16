using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class DepartmentType : ObjectGraphType<Department>
    {
        public DepartmentType()
        {
            Name = "Department";

            Field(x => x.DepartmentId).Description("Id");
            Field(x => x.Name).Description("Name");
        }
    }
}
