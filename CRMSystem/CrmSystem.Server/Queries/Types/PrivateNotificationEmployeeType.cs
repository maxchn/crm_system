using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class PrivateNotificationEmployeeType : ObjectGraphType<PrivateNotificationEmployee>
    {
        public PrivateNotificationEmployeeType()
        {
            Name = "PrivateNotificationEmployee";

            Field(n => n.Id);
            Field(n => n.PrivateNotificationId);
            Field<PrivateNotificationType>("PrivateNotification");
            Field(n => n.EmployeeId);
            Field<ApplicationUserType>("Employee");
            Field(n => n.CompanyId);
            Field<CompanyType>("Company");
        }
    }
}