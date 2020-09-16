using CrmSystem.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Interfaces
{
    public interface ITaskExtRepository
    {
        Task<IList<MTask>> GetAllTasksAsAuthor(int companyId, string userId);
        Task<int> GetAllTasksAsAuthorCount(int companyId, string userId);

        Task<IList<MTask>> GetAllTasksAsCoExecutor(int companyId, string userId);
        Task<int> GetAllTasksAsCoExecutorCount(int companyId, string userId);

        Task<IList<MTask>> GetAllTasksAsObserver(int companyId, string userId);
        Task<int> GetAllTasksAsObserverCount(int companyId, string userId);

        Task<IList<MTask>> GetAllTasksAsResponsible(int companyId, string userId);
        Task<int> GetAllTasksAsResponsibleCount(int companyId, string userId);

        Task<IList<MTask>> GetAllOverdueTasks(int companyId, string userId);
        Task<IList<MTask>> GetAllCompletedTasks(int companyId, string userId);
        Task<ExecutionResult> MarkExecution(int taskId, bool status, ApplicationUser user);
        Task MarkCompleted(int taskId, string userId);
        Task CreateTaskNotification(MTask task, ApplicationUser author, TaskNotificationType type, Company company);

        Task<int> GetCountIssuedTasksPerMonth(int companyId);
        Task<int> GetCountTasksCompletedPerMonth(int companyId);
        Task<int> GetCountAllTasksCompletedPerMonth(int companyId);
        Task<int> GetCountTasksOutstandingPerMonth(int companyId);
    }
}