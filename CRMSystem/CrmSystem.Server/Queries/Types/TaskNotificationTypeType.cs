using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskNotificationTypeType : EnumerationGraphType<DAL.Entities.TaskNotificationType>
    {
        public TaskNotificationTypeType()
        {
            Name = "TaskNotificationType";
        }
    }
}