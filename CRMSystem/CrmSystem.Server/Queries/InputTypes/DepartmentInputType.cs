using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class DepartmentInputType : InputObjectGraphType
    {
        public DepartmentInputType()
        {
            Name = "DepartmentInputType";

            Field<IntGraphType>("DepartmentId");
            Field<NonNullGraphType<StringGraphType>>("Name");
        }
    }
}