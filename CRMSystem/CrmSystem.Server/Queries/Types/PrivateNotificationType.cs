using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class PrivateNotificationType : ObjectGraphType<DAL.Entities.PrivateNotification>
    {
        public PrivateNotificationType()
        {
            Name = "PrivateNotification";

            Field(n => n.PrivateNotificationId);
            Field<ApplicationUserType>("Author");
            Field(n => n.Body);
            Field<DateTimeGraphType>("DateTime");
        }
    }
}
