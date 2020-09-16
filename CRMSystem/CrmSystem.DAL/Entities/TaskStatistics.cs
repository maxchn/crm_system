namespace CrmSystem.DAL.Entities
{
    public class TaskStatistics
    {
        #region Tasks Statistics For Current Employee

        // Количество задач (роль автор)
        public int CountTasksAsAuthor { get; set; }

        // Количество задач (роль соисполнитель)
        public int CountTasksAsCoExecutor { get; set; }

        // Количество задач (роль наблюдатель)
        public int CountTasksAsObserver { get; set; }

        // Количество задач (роль ответственный за выполнение)
        public int CountTasksAsResponsible { get; set; }

        // Количество выполненых пользователем задач
        public int CountCompletedTasks { get; set; }

        // Количество просроченых пользователе задач
        public int CountOverdueTasks { get; set; }

        #endregion

        #region Tasks Statistics For Current Company

        // Количество выданных задач в месяц
        public int CountIssuedTasksPerMonth { get; set; }

        // Количество выполненных задач за месяц
        public int CountTasksCompletedPerMonth { get; set; }

        // Предполагаемое количество всех задач выполненных за месяц
        public int CountAllTasksCompletedPerMonth { get; set; }

        // Количество задач за месяц требующих выполнения
        public int CountTasksOutstandingPerMonth { get; set; }

        #endregion
    }
}