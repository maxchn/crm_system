using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class PrivateNotificationEmployeeInputType : InputObjectGraphType
    {
        public PrivateNotificationEmployeeInputType()
        {
            Name = "PrivateNotificationEmployeeInput";

            Field<IntGraphType>("Id");
            Field<IntGraphType>("PrivateNotificationId");
            Field<NonNullGraphType<StringGraphType>>("EmployeeId");
            Field<NonNullGraphType<IntGraphType>>("CompanyId");
        }
    }
}