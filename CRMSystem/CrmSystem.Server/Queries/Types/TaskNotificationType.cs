using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskNotificationType : ObjectGraphType<TaskNotification>
    {
        public TaskNotificationType()
        {
            Name = "TaskNotification";

            Field<ApplicationUserType>("Author");
            Field<TaskType>("Task");
            Field<TaskNotificationTypeType>("Type");
            Field<DateTimeGraphType>("DateTime");
        }
    }
}