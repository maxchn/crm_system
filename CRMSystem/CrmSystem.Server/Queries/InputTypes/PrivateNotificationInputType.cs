using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class PrivateNotificationInputType : InputObjectGraphType
    {
        public PrivateNotificationInputType()
        {
            Name = "PrivateNotificationInput";

            Field<IntGraphType>("PrivateNotificationId");
            Field<NonNullGraphType<StringGraphType>>("AuthorId");
            Field<ApplicationUserInputType>("Author");
            Field<NonNullGraphType<StringGraphType>>("Body");
            Field<DateTimeGraphType>("DateTime");
        }
    }
}