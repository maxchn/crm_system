using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Repositories
{
    public class NpsqlTaskRepository : IGenericRepository<MTask>, ITaskExtRepository
    {
        private NpgsqlDbContext _context;

        public NpsqlTaskRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var task = await FindById(id);

            if (task is null)
                throw new ArgumentException("Task with the specified ID not found!!!");

            // удаление тегов которые относятся только к этой задачи
            // если в задачи есть теги
            if (task.TaskTags != null)
            {
                // проходим по списку тегов которые относятся к текущей задачи
                foreach (var tag in task.TaskTags)
                {
                    var searchTag = await _context.Tags.Include(i => i.TaskTags).FirstOrDefaultAsync(t => t.TagId == tag.TagId);

                    // если тег найден
                    if (searchTag != null)
                    {
                        // если у тега есть связь только с текущей задачей то удаляем его
                        if (searchTag.TaskTags != null && searchTag.TaskTags.Count == 1)
                            _context.Tags.Remove(searchTag);
                    }
                }
            }

            _context.Tasks.Remove(task);
        }

        public Task<IEnumerable<MTask>> Find(Expression<Func<MTask, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MTask>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<MTask> FindById(object id)
        {
            var task = await _context.Tasks
                .Include(x => x.Author)
                .Include(x => x.Company)
                .Include(x => x.ResponsiblesForExecution)
                .Include(x => x.CoExecutors)
                .Include(x => x.Observers)
                .Include(x => x.TaskTags)
                .Include(x => x.AttachedFiles)
                .FirstOrDefaultAsync(x => x.TaskId == (int)id);

            if (task.ResponsiblesForExecution.Count > 0)
            {
                for (int i = 0; i < task.ResponsiblesForExecution.Count; i++)
                {
                    task.ResponsiblesForExecution[i].User =
                        await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(task.ResponsiblesForExecution[i].Id));
                }
            }

            if (task.CoExecutors.Count > 0)
            {
                for (int i = 0; i < task.CoExecutors.Count; i++)
                {
                    task.CoExecutors[i].User =
                        await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(task.CoExecutors[i].Id));
                }
            }

            if (task.Observers.Count > 0)
            {
                for (int i = 0; i < task.Observers.Count; i++)
                {
                    task.Observers[i].User =
                        await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(task.Observers[i].Id));
                }
            }

            if (task.TaskTags.Count > 0)
            {
                for (int i = 0; i < task.TaskTags.Count; i++)
                {
                    task.TaskTags[i].Tag =
                        await _context.Tags.FirstOrDefaultAsync(x => x.TagId == task.TaskTags[i].TagId);
                }
            }

            return task;
        }

        public async Task<IList<MTask>> GetAllTasksAsAuthor(int companyId, string userId)
        {
            return await _context.Tasks
                .Include(x => x.Author)
                .Include(x => x.Company)
                .Where(x => x.Company.CompanyId == companyId && x.Author.Id.Equals(userId))
                .ToListAsync();
        }

        public async Task<int> GetAllTasksAsAuthorCount(int companyId, string userId)
        {
            return await _context.Tasks
                .Include(x => x.Author)
                .Include(x => x.Company)
                .Where(x => x.Company.CompanyId == companyId && x.Author.Id.Equals(userId))
                .CountAsync();
        }

        public async Task<IList<MTask>> GetAllTasksAsCoExecutor(int companyId, string userId)
        {
            var tasks = await _context.CoExecutors
                .Include(x => x.Task)
                .Include(x => x.Task.Company)
                .Include(x => x.Task.Author)
                .Where(x => x.Id.Equals(userId) && x.Task.Company.CompanyId == companyId && x.Task.FinalPerformerId == null)
                .ToListAsync();

            return tasks.Select(x => x.Task).ToList();
        }

        public async Task<int> GetAllTasksAsCoExecutorCount(int companyId, string userId)
        {
            return await _context.CoExecutors
                .Include(x => x.Task)
                .Include(x => x.Task.Company)
                .Include(x => x.Task.Author)
                .Where(x => x.Id.Equals(userId) && x.Task.Company.CompanyId == companyId && x.Task.FinalPerformerId == null)
                .CountAsync();
        }

        public async Task<IList<MTask>> GetAllTasksAsObserver(int companyId, string userId)
        {
            var tasks = await _context.Observers
                .Include(o => o.Task)
                .Include(o => o.Task.Company)
                .Include(o => o.Task.Author)
                .Where(o => o.Id.Equals(userId) && o.Task.Company.CompanyId == companyId)
                .ToListAsync();

            return tasks.Select(o => o.Task).ToList();
        }

        public async Task<int> GetAllTasksAsObserverCount(int companyId, string userId)
        {
            return await _context.Observers
                .Include(o => o.Task)
                .Include(o => o.Task.Company)
                .Include(o => o.Task.Author)
                .Where(o => o.Id.Equals(userId) && o.Task.Company.CompanyId == companyId)
                .CountAsync();
        }

        public async Task<IList<MTask>> GetAllTasksAsResponsible(int companyId, string userId)
        {
            var task = await _context.ResponsibleForExecutions
                .Include(r => r.Task)
                .Include(r => r.Task.Company)
                .Include(o => o.Task.Author)
                .Where(r => r.Id.Equals(userId) &&
                            r.Task.Company.CompanyId == companyId &&
                            r.Task.FinalPerformerId == null &&
                            (r.Task.Deadline < DateTime.Now && r.Task.FinalPerformerId != null || r.Task.Deadline >= DateTime.Now && r.Task.FinalPerformerId == null))
                .ToListAsync();

            return task.Select(r => r.Task).ToList();
        }

        public async Task<int> GetAllTasksAsResponsibleCount(int companyId, string userId)
        {
            return await _context.ResponsibleForExecutions
                .Include(r => r.Task)
                .Include(r => r.Task.Company)
                .Include(o => o.Task.Author)
                .Where(r => r.Id.Equals(userId) &&
                            r.Task.Company.CompanyId == companyId &&
                            r.Task.FinalPerformerId == null &&
                            (r.Task.Deadline < DateTime.Now && r.Task.FinalPerformerId != null || r.Task.Deadline >= DateTime.Now && r.Task.FinalPerformerId == null))
                .CountAsync();
        }

        public async Task<IList<MTask>> GetAllOverdueTasks(int companyId, string userId)
        {
            List<MTask> tasks = new List<MTask>();

            tasks.AddRange(await _context.ResponsibleForExecutions
                .Include(r => r.Task)
                .Include(r => r.Task.Company)
                .Include(o => o.Task.Author)
                .Where(r => r.Id.Equals(userId) &&
                            r.Task.Company.CompanyId == companyId)
                .Select(r => r.Task)
                .ToListAsync());

            tasks.AddRange(await _context.CoExecutors
                .Include(c => c.Task)
                .Include(c => c.Task.Company)
                .Include(o => o.Task.Author)
                .Where(c => c.Id.Equals(userId) && c.Task.Company.CompanyId == companyId)
                .Select(c => c.Task)
                .ToListAsync());

            return tasks.Where(t => t.Deadline < DateTime.Now && t.FinalPerformerId == null)
                .Distinct()
                .ToList();
        }

        public async Task<IList<MTask>> GetAllCompletedTasks(int companyId, string userId)
        {
            List<MTask> tasks = new List<MTask>();

            tasks.AddRange(await _context.ResponsibleForExecutions
                    .Include(r => r.Task)
                    .Include(r => r.Task.Company)
                    .Include(o => o.Task.Author)
                    .Where(r => r.Id.Equals(userId) &&
                                r.Task.Company.CompanyId == companyId)
                    .Select(r => r.Task)
                    .ToListAsync());

            tasks.AddRange(await _context.CoExecutors
                    .Include(c => c.Task)
                    .Include(c => c.Task.Company)
                    .Include(o => o.Task.Author)
                    .Where(c => c.Id.Equals(userId) && c.Task.Company.CompanyId == companyId)
                    .Select(c => c.Task)
                    .ToListAsync());

            return tasks.Where(t => !(t.Deadline < DateTime.Now && t.FinalPerformerId == null) && t.FinalPerformerId != null)
                .Distinct()
                .ToList();
        }

        public async Task Insert(MTask entity)
        {
            entity.CreatedDate = DateTime.Now;

            if (entity.Author != null)
            {
                ApplicationUser author = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(entity.Author.Id));
                entity.Author = author;
            }

            if (entity.Company != null)
            {
                Company company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == entity.Company.CompanyId);
                entity.Company = company;
            }

            if (entity.TaskTags != null)
            {
                for (int i = 0; i < entity.TaskTags.Count; i++)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(entity.TaskTags[i].Tag.Name.ToLower()));

                    entity.TaskTags[i].Tag = tag is null ? entity.TaskTags[i].Tag : tag;
                    entity.TaskTags[i].TaskId = entity.TaskId;
                }
            }

            await _context.Tasks.AddAsync(entity);
            await _context.SaveChangesAsync();

            if (entity.ResponsiblesForExecution != null)
            {
                for (int i = 0; i < entity.ResponsiblesForExecution.Count; i++)
                {
                    entity.ResponsiblesForExecution[i].TaskId = entity.TaskId;
                }
            }

            if (entity.CoExecutors != null)
            {
                for (int i = 0; i < entity.CoExecutors.Count; i++)
                {
                    entity.CoExecutors[i].TaskId = entity.TaskId;
                }
            }

            if (entity.Observers != null)
            {
                for (int i = 0; i < entity.Observers.Count; i++)
                {
                    entity.Observers[i].TaskId = entity.TaskId;
                }
            }


        }

        public async Task Update(MTask entityToUpdate)
        {
            var task = await FindById(entityToUpdate.TaskId);

            if (task is null)
                throw new ArgumentException("Task with the specified ID not found!!!");

            _context.Tasks.Attach(task);

            foreach (var file in task.AttachedFiles)
            {
                // если в списке отсутствует прикрепленный файл то удаляем его из списка прикрепленных
                if (entityToUpdate.AttachedFiles.FirstOrDefault(f => f.AttachedFileId == file.AttachedFileId) is null)
                {
                    var attachedFile = await _context.AttachedFiles.FirstOrDefaultAsync(f => f.AttachedFileId == file.AttachedFileId);
                    _context.AttachedFiles.Remove(attachedFile);

                    var taskAttachedFile = await _context.TaskAttachedFiles.FirstOrDefaultAsync(f => f.AttachedFileId == file.AttachedFileId);
                    _context.TaskAttachedFiles.Remove(taskAttachedFile);
                }
            }

            foreach (var file in entityToUpdate.AttachedFiles)
            {
                if (file.AttachedFileId <= 0)
                {
                    file.Task = task;
                    task.AttachedFiles.Add(file);
                }
            }

            task.Name = entityToUpdate.Name;
            task.Body = entityToUpdate.Body;
            task.Deadline = entityToUpdate.Deadline;
            task.IsImportant = entityToUpdate.IsImportant;

            task.CoExecutors.Clear();
            task.CoExecutors = entityToUpdate.CoExecutors;

            task.Observers.Clear();
            task.Observers = entityToUpdate.Observers;

            task.ResponsiblesForExecution.Clear();
            task.ResponsiblesForExecution = entityToUpdate.ResponsiblesForExecution;


            if (entityToUpdate.TaskTags != null)
            {
                foreach (var taskTag in task.TaskTags)
                {
                    // Находим тег в БД
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagId == taskTag.TagId);

                    int taskCount = _context.TaskTags.Where(t => t.TagId == taskTag.TagId).Count();
                    int companyCount = _context.CompanyTags.Where(t => t.TagId == taskTag.TagId).Count();

                    // Если у тега есть только одна связь с текущей задачей
                    // и одна связь с компанией
                    // и при обновлении этот тег не используется
                    // то удаляем этот тег
                    if (taskCount <= 1 &&
                        companyCount <= 1 &&
                        taskTag.TaskId == entityToUpdate.TaskId &&
                        entityToUpdate.TaskTags.FirstOrDefault(t => t.Tag.Name.Equals(tag.Name)) is null)
                    {
                        TaskTag searchTaskTag = await _context.TaskTags.FirstOrDefaultAsync(t => t.Id == taskTag.Id);

                        if (searchTaskTag != null)
                        {
                            _context.TaskTags.Remove(searchTaskTag);
                        }

                        CompanyTag companyTag = await _context.CompanyTags.FirstOrDefaultAsync(t => t.TagId == taskTag.TagId);

                        _context.Tags.Remove(tag);
                    }
                    else
                    {
                        _context.TaskTags.Remove(taskTag);
                    }
                }

                await _context.SaveChangesAsync();
                task.TaskTags.Clear();

                for (int i = 0; i < entityToUpdate.TaskTags.Count; i++)
                {
                    // Ищем тег
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(entityToUpdate.TaskTags[i].Tag.Name.ToLower()));

                    // Если тег найден
                    if (tag != null)
                    {
                        // Связываем тег с задачей
                        task.TaskTags.Add(new TaskTag
                        {
                            TagId = tag.TagId,
                            TaskId = task.TaskId
                        });
                    }
                    else
                    {
                        task.TaskTags.Add(entityToUpdate.TaskTags[i]);
                    }

                    var companyTag = await _context.CompanyTags.FirstOrDefaultAsync(ct => ct.CompanyId == task.Company.CompanyId && ct.TagId == tag.TagId);

                    // Если указанный тег не связан с текущей компанией то тогда связываем их
                    if (companyTag is null)
                    {
                        await _context.CompanyTags.AddAsync(new CompanyTag
                        {
                            CompanyId = task.Company.CompanyId,
                            TagId = tag.TagId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ExecutionResult> MarkExecution(int taskId, bool status, ApplicationUser user)
        {
            var task = await FindById(taskId);

            if (task is null)
                throw new ArgumentException("Task with the specified ID not found!!!");

            if (status)
            {
                task.StartExecution = DateTime.Now;
                task.IsExecution = true;
                await CreateTaskNotification(task, user, TaskNotificationType.Doing, task.Company);
            }
            else
            {
                task.EndExecution = DateTime.Now;
                task.IsExecution = false;

                TimeSpan timeSpan = task.EndExecution - task.StartExecution;
                task.TotalTime += timeSpan.TotalMinutes;
                await CreateTaskNotification(task, user, TaskNotificationType.ToDo, task.Company);
            }

            await _context.SaveChangesAsync();

            return new ExecutionResult { IsExecution = task.IsExecution, TotalTime = task.TotalTime };
        }

        public async Task MarkCompleted(int taskId, string userId)
        {
            var task = await FindById(taskId);

            if (task is null)
                throw new ArgumentException("Task with the specified ID not found!!!");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));

            if (user is null)
                throw new ArgumentException("User with the specified ID not found!!!");

            task.FinalPerformerId = user.Id;
            task.IsExecution = false;
            task.EndExecution = DateTime.Now;

            await CreateTaskNotification(task, user, TaskNotificationType.Closed, task.Company);
        }

        public async Task CreateTaskNotification(MTask task, ApplicationUser author, TaskNotificationType type, Company company)
        {
            HashSet<string> users = new HashSet<string>();

            var newNotification = new TaskNotification();
            newNotification.Author = author;
            newNotification.Task = task;
            newNotification.Type = type;
            newNotification.DateTime = DateTime.Now;

            await _context.TaskNotifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();

            if (task.CoExecutors != null)
            {
                foreach (var coExecutor in task.CoExecutors)
                {
                    // Проверяем что для указаного сотрудника еще не было 
                    // сформированно уведомление
                    if (!users.Contains(coExecutor.Id))
                    {
                        var taskNotificationEmployee = CreateNotificationEmployee(newNotification.TaskNotificationId, coExecutor.Id, company.CompanyId);
                        await _context.TaskNotificationEmployees.AddAsync(taskNotificationEmployee);

                        // заносим указаного сотрудника в список
                        // сотрудников которым было сформированно уведомление
                        users.Add(coExecutor.Id);
                    }
                }
            }

            if (task.Observers != null)
            {
                foreach (var observer in task.Observers)
                {
                    if (!users.Contains(observer.Id))
                    {
                        var taskNotificationEmployee = CreateNotificationEmployee(newNotification.TaskNotificationId, observer.Id, company.CompanyId);
                        await _context.TaskNotificationEmployees.AddAsync(taskNotificationEmployee);

                        users.Add(observer.Id);
                    }
                }
            }

            if (task.ResponsiblesForExecution != null)
            {
                foreach (var responsibleForExecution in task.ResponsiblesForExecution)
                {
                    if (!users.Contains(responsibleForExecution.Id))
                    {
                        var taskNotificationEmployee = CreateNotificationEmployee(newNotification.TaskNotificationId, responsibleForExecution.Id, company.CompanyId);
                        await _context.TaskNotificationEmployees.AddAsync(taskNotificationEmployee);

                        users.Add(responsibleForExecution.Id);
                    }
                }
            }

            if (task.Author != null && !users.Contains(task.Author.Id))
            {
                var taskNotificationEmployee = CreateNotificationEmployee(newNotification.TaskNotificationId, task.Author.Id, company.CompanyId);
                await _context.TaskNotificationEmployees.AddAsync(taskNotificationEmployee);
            }
        }

        private TaskNotificationEmployee CreateNotificationEmployee(int taskNotificationId, string employeeId, int companyId)
        {
            return new TaskNotificationEmployee
            {
                TaskNotificationId = taskNotificationId,
                EmployeeId = employeeId,
                CompanyId = companyId
            };
        }

        public async Task<int> GetCountIssuedTasksPerMonth(int companyId)
        {
            return await _context.Tasks
                .Include(c => c.Company)
                .Where(t => t.Company.CompanyId == companyId &&
                       t.CreatedDate.Month == DateTime.Now.Month &&
                       t.CreatedDate.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<int> GetCountTasksCompletedPerMonth(int companyId)
        {
            return await _context.Tasks
                .Include(c => c.Company)
                .Where(t => t.Company.CompanyId == companyId &&
                       t.EndExecution.Month == DateTime.Now.Month &&
                       t.EndExecution.Year == DateTime.Now.Year &&
                       t.FinalPerformerId != null && t.IsExecution == false)
                .CountAsync();
        }

        public async Task<int> GetCountAllTasksCompletedPerMonth(int companyId)
        {
            return await _context.Tasks
                .Include(c => c.Company)
                .Where(t => t.Company.CompanyId == companyId &&
                       t.Deadline.Month == DateTime.Now.Month &&
                       t.Deadline.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<int> GetCountTasksOutstandingPerMonth(int companyId)
        {
            return await _context.Tasks
                .Include(c => c.Company)
                .Where(t => t.Company.CompanyId == companyId &&
                       t.Deadline.Month == DateTime.Now.Month &&
                       t.Deadline.Year == DateTime.Now.Year &&
                       t.FinalPerformerId == null)
                .CountAsync();
        }
    }
}