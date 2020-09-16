using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class CompanyNotificationTypeType : EnumerationGraphType<DAL.Entities.CompanyNotificationType>
    {
        public CompanyNotificationTypeType()
        {
            Name = "CompanyNotificationType";
        }
    }
}