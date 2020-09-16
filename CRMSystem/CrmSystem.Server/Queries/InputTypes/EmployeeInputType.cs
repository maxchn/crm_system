using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class EmployeeInputType : InputObjectGraphType
    {
        public EmployeeInputType()
        {
            Name = "EmployeeInput";

            Field<NonNullGraphType<StringGraphType>>("Email");
            Field<NonNullGraphType<StringGraphType>>("LastName");
            Field<NonNullGraphType<StringGraphType>>("FirstName");
            Field<NonNullGraphType<StringGraphType>>("Patronymic");
            Field<NonNullGraphType<PositionInputType>>("Position");
            Field<NonNullGraphType<DepartmentInputType>>("Department");
        }
    }
}