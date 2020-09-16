using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class CompanyNotificationType : ObjectGraphType<DAL.Entities.CompanyNotification>
    {
        public CompanyNotificationType()
        {
            Name = "CompanyNotification";

            Field(x => x.CompanyNotificationId);
            Field<ApplicationUserType>("Author");
            Field(x => x.CompanyId);
            Field<ApplicationUserType>("NewEmployee");
            Field(x => x.Body);
            Field<CompanyNotificationTypeType>("Type");
            Field<DateTimeGraphType>("DateTime");
        }
    }
}