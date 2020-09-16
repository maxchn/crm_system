namespace CrmSystem.DAL.Entities
{
    public class TaskNotificationEmployee
    {
        public int TaskNotificationEmployeeId { get; set; }

        public int TaskNotificationId { get; set; }
        public TaskNotification TaskNotification { get; set; }

        public string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}