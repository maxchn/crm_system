using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskStatisticsType : ObjectGraphType<TaskStatistics>
    {
        public TaskStatisticsType()
        {
            Name = "TaskStatistics";

            Field(x => x.CountTasksAsAuthor);
            Field(x => x.CountTasksAsCoExecutor);
            Field(x => x.CountTasksAsObserver);
            Field(x => x.CountTasksAsResponsible);
            Field(x => x.CountCompletedTasks);
            Field(x => x.CountOverdueTasks);

            Field(x => x.CountIssuedTasksPerMonth);
            Field(x => x.CountTasksCompletedPerMonth);
            Field(x => x.CountAllTasksCompletedPerMonth);
            Field(x => x.CountTasksOutstandingPerMonth);
        }
    }
}