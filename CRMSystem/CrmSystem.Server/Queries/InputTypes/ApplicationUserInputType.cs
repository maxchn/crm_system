using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class ApplicationUserInputType : InputObjectGraphType
    {
        public ApplicationUserInputType()
        {
            Name = "ApplicationUserInput";

            Field<NonNullGraphType<StringGraphType>>("Id");
            Field<NonNullGraphType<StringGraphType>>("LastName");
            Field<NonNullGraphType<StringGraphType>>("FirstName");
            Field<NonNullGraphType<StringGraphType>>("Patronymic");
            Field<NonNullGraphType<DateGraphType>>("DateOfBirth");
            Field<NonNullGraphType<GenderInputType>>("Gender");
            Field<NonNullGraphType<PositionInputType>>("Position");
            Field<NonNullGraphType<DepartmentInputType>>("Department");
            Field<NonNullGraphType<ListGraphType<PhoneInputType>>>("Phones");
        }
    }
}