using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.GraphQL;
using CrmSystem.Server.Queries.InputTypes;
using CrmSystem.Server.Queries.Types;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CompanyNotificationType = CrmSystem.Server.Queries.Types.CompanyNotificationType;
using TaskNotificationType = CrmSystem.Server.Queries.Types.TaskNotificationType;
using TaskType = CrmSystem.Server.Queries.Types.TaskType;

namespace CrmSystem.Server.Queries
{
    public class AppQuery : ObjectGraphType
    {
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppQuery(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            this.AuthorizeWith("Authorized");

            Field<ApplicationUserType>(
                name: "user",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                resolve: GetUserById
            );

            Field<BooleanGraphType>(
                name: "userProfileIsFull",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                resolve: GetUserProfileIsFull
            );

            Field<CompanyType>(
                name: "employeeCompany",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "employeeId" }),
                resolve: GetEmployeeCompany
            );

            Field<ListGraphType<ApplicationUserType>>(
                name: "users",
                resolve: GetAllUsers
            );

            Field<CompanyType>(
                name: "company",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: GetCompany
            );

            Field<BooleanGraphType>(
                name: "getPermissionOnUpdatingCompanyData",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: GetPermissionOnUpdatingCompanyData
            );

            Field<ListGraphType<ApplicationUserType>>(
                name: "employees",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: GetAllEmployees
            );

            Field<TaskType>(
                name: "task",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: GetTaskDetails
            );

            Field<ListGraphType<TaskType>>(
                name: "tasks",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<TaskTypeType>> { Name = "taskType" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: GelAllTask
            );

            Field<BooleanGraphType>(
                name: "getAccessOnReopenTask",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" }),
                resolve: GetAccessOnReopenTask
            );

            Field<ListGraphType<ChatType>>(
                name: "chats",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: GetAllChats
            );

            Field<ListGraphType<TaskCommentType>>(
                name: "comments",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" }),
                resolve: GetAllTaskComments
            );

            //Field<BooleanGraphType>(
            //    name: "getPermissionOnTaskComment",
            //    arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskCommentId" }),
            //    resolve: GetPermissionOnTaskComment
            //);

            Field<ListGraphType<CalendarEventType>>(
                name: "calendarEvents",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: GetAllCalendarEvents
            );

            Field<ListGraphType<ShortLinkType>>(
                name: "userShortLinks",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: GetAllUserShortLinks
            );

            Field<ListGraphType<ShortLinkType>>(
                name: "companyShortLinks",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetAllCompanyShortLinks
            );

            Field<ListGraphType<TaskAttachedFileType>>(
                name: "taskAttachedFiles",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" }),
                resolve: GetAllTaskAttachedFiles
            );

            Field<ListGraphType<ApplicationUserType>>(
                name: "birthDayNotifications",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetAllBirthDayNotifications
            );

            Field<ListGraphType<TaskNotificationType>>(
                name: "taskNotifications",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetAllTaskNotifications
            );

            Field<ListGraphType<CompanyNotificationType>>(
                name: "companyNotifications",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetAllCompanyNotifications
            );

            Field<ListGraphType<PrivateNotificationType>>(
                name: "privateNotifications",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetAllPrivateNotifications
            );

            Field<PrivateNotificationType>(
                name: "privateNotification",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: GetPrivateNotification
            );

            Field<ListGraphType<PrivateNotificationEmployeeType>>(
                name: "privateNotificationEmployees",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "notificationId" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetPrivateNotificationEmployees
            );

            Field<BooleanGraphType>(
                name: "getPermissionAsOwner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetPermissionAsOwner
            );

            //Field<IntGraphType>(
            //    name: "getTaskCount",
            //    arguments: new QueryArguments(new QueryArgument<NonNullGraphType<TaskTypeType>> { Name = "taskType" },
            //            new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
            //    resolve: GetTaskCount
            //);

            Field<TaskStatisticsType>(
                name: "getTaskStatistics",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: GetTasksStatistics
            );

            //Field<BooleanGraphType>(
            //    name: "getPermissionOnPrivateNotification",
            //    arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "notificationId" },
            //            ),
            //    resolve: GetPermissionOnPrivateNotification
            //);
        }

        #region User(Employee) block

        private async Task<ApplicationUser> GetUserById(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<string>("id");
            return await _unitOfWork.User.FindById(id);
        }

        private async Task<bool> GetUserProfileIsFull(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<string>("id");
            var user = await _unitOfWork.User.FindById(id);
            return user.IsFullProfile;
        }

        private async Task<IEnumerable<ApplicationUser>> GetAllUsers(ResolveFieldContext<object> context)
        {
            return await _unitOfWork.User.FindAll();
        }

        #endregion

        #region Company block

        private async Task<Company> GetEmployeeCompany(ResolveFieldContext<object> context)
        {
            var employeeId = context.GetArgument<string>("employeeId");
            return (await _unitOfWork.Company.FindByEmployee(employeeId))?.FirstOrDefault();
        }

        private async Task<Company> GetCompany(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");
            return await _unitOfWork.Company.FindById(id);
        }

        private async Task<bool> GetPermissionOnUpdatingCompanyData(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");

            try
            {
                var company = await _unitOfWork.Company.FindById(id);

                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                return company.Owner.Id.Equals(user.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<IList<ApplicationUser>> GetAllEmployees(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");
            var employees = await _unitOfWork.Company.GetAllEmployees(id);
            return employees;
        }

        #endregion

        #region Task block

        private async Task<MTask> GetTaskDetails(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");
            var task = await _unitOfWork.Task.FindById(id);
            return task;
        }

        private async Task<IList<MTask>> GelAllTask(ResolveFieldContext<object> context)
        {
            DAL.Entities.TaskType taskType = context.GetArgument<DAL.Entities.TaskType>("taskType");
            int companyId = context.GetArgument<int>("companyId");
            string userId = context.GetArgument<string>("userId");

            IList<MTask> tasks = null;

            switch (taskType)
            {
                case DAL.Entities.TaskType.AsAuthor:
                    tasks = await _unitOfWork.Task.GetAllTasksAsAuthor(companyId, userId);
                    break;
                case DAL.Entities.TaskType.AsCoExecutor:
                    tasks = await _unitOfWork.Task.GetAllTasksAsCoExecutor(companyId, userId);
                    break;
                case DAL.Entities.TaskType.AsObserver:
                    tasks = await _unitOfWork.Task.GetAllTasksAsObserver(companyId, userId);
                    break;
                case DAL.Entities.TaskType.AsResponsible:
                    tasks = await _unitOfWork.Task.GetAllTasksAsResponsible(companyId, userId);
                    break;
                case DAL.Entities.TaskType.Completed:
                    tasks = await _unitOfWork.Task.GetAllCompletedTasks(companyId, userId);
                    break;
                case DAL.Entities.TaskType.Overdue:
                    tasks = await _unitOfWork.Task.GetAllOverdueTasks(companyId, userId);
                    break;
            }

            return tasks;
        }

        private async Task<bool> GetAccessOnReopenTask(ResolveFieldContext<object> context)
        {
            int taskId = context.GetArgument<int>("taskId");

            try
            {
                var task = await _unitOfWork.Task.FindById(taskId);
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                return task.FinalPerformerId != null && task.Author.Id == user.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion

        #region Chat block

        private async Task<IList<Chat>> GetAllChats(ResolveFieldContext<object> context)
        {
            string userId = context.GetArgument<string>("userId");
            int companyId = context.GetArgument<int>("companyId");

            var user = await _unitOfWork.User.FindById(userId);
            List<Chat> chats = (await _unitOfWork.Chat.Find(c => c.ChatParticipants.Exists(p => p.Id.Equals(user.Id)))).ToList();

            if (chats != null)
            {
                chats.ForEach(c => c.ChatParticipants.ForEach(u =>
                {
                    u.Chat = null;

                    if (u.User != null)
                        u.User.ChatParticipants = null;
                }));
            }

            return chats;
        }

        private async Task<IEnumerable<TaskComment>> GetAllTaskComments(ResolveFieldContext<object> context)
        {
            int taskId = context.GetArgument<int>("taskId");

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                var comments = await _unitOfWork.TaskComment.Find(x => x.Task.TaskId == taskId);

                foreach (var comment in comments)
                {
                    comment.IsAccessOnDeleting = user.Id.Equals(comment.AuthorId);
                }

                return comments;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<TaskAttachedFile>> GetAllTaskAttachedFiles(ResolveFieldContext<object> context)
        {
            int taskId = context.GetArgument<int>("taskId");

            try
            {
                IEnumerable<TaskAttachedFile> attachedFiles = await _unitOfWork.TaskAttachedFile.Find(f => f.TaskId == taskId);
                return attachedFiles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }


        #endregion

        #region Calendar Event block

        private async Task<IEnumerable<CalendarEvent>> GetAllCalendarEvents(ResolveFieldContext<object> context)
        {
            string userId = context.GetArgument<string>("userId");

            try
            {
                var calendarEvents = await _unitOfWork.CalendarEvent.Find(e => e.AuthorId.Equals(userId));
                return calendarEvents;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion

        #region Short Link block

        private async Task<IEnumerable<ShortLink>> GetAllUserShortLinks(ResolveFieldContext<object> context)
        {
            string userId = context.GetArgument<string>("userId");

            try
            {
                var shortLinks = await _unitOfWork.ShortLink.Find(l => userId.Equals(l.OwnerId));
                return shortLinks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<ShortLink>> GetAllCompanyShortLinks(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                var shortLinks = await _unitOfWork.ShortLink.Find(l => companyId.Equals(l.CompanyId));
                return shortLinks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion

        #region Notification Block

        private async Task<IEnumerable<ApplicationUser>> GetAllBirthDayNotifications(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                var employees = await _unitOfWork.Company.GetAllEmployees(companyId);

                DateTime today = DateTime.Now;
                employees = employees.Where(e => e.DateOfBirth.Day == today.Day && e.DateOfBirth.Month == today.Month).ToList();

                return employees;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<TaskNotification>> GetAllTaskNotifications(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                var notificationIds = (await _unitOfWork.TaskNotificationEmployee.Find(n =>
                    n.EmployeeId.Equals(user.Id) &&
                    n.CompanyId == companyId)).Select(n => n.TaskNotificationId);

                var notification = await _unitOfWork.TaskNotification.Find(n => notificationIds.Contains(n.TaskNotificationId));
                return notification;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<CompanyNotification>> GetAllCompanyNotifications(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                return await _unitOfWork.CompanyNotification.Find(n => n.CompanyId == companyId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<PrivateNotification>> GetAllPrivateNotifications(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                var privateNotificationIds = (await _unitOfWork.PrivateNotificationEmployee.Find(n => n.EmployeeId.Equals(user.Id) && n.CompanyId == companyId))
                        .Select(n => n.PrivateNotificationId);

                var notifications = await _unitOfWork.PrivateNotification.Find(n => privateNotificationIds.Contains(n.PrivateNotificationId));

                return notifications;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<PrivateNotification> GetPrivateNotification(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                var privateNotification = await _unitOfWork.PrivateNotification.FindById(id);

                if (!privateNotification.AuthorId.Equals(user.Id))
                    throw new Exception("Отказано в доступе!");

                return privateNotification;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<IEnumerable<PrivateNotificationEmployee>> GetPrivateNotificationEmployees(ResolveFieldContext<object> context)
        {
            int notificationId = context.GetArgument<int>("notificationId");
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                var notificationEmployees = await _unitOfWork.PrivateNotificationEmployee.Find(n => n.PrivateNotificationId == notificationId && n.CompanyId == companyId);
                return notificationEmployees;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<bool> GetPermissionAsOwner(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                var company = await _unitOfWork.Company.FindById(companyId);
                return company.Owner.Id.Equals(user.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        //private async Task<bool> GetPermissionOnPrivateNotification(ResolveFieldContext<object> context)
        //{
        //    int notificationId = context.GetArgument<int>("notificationId");

        //    try
        //    {
        //        ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

        //        var notification = await _unitOfWork.PrivateNotification.FindById(notificationId);
        //        return notification.AuthorId.Equals(user.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        return false;
        //    }
        //}

        #endregion

        #region Statistics block 

        private async Task<TaskStatistics> GetTasksStatistics(ResolveFieldContext<object> context)
        {
            int companyId = context.GetArgument<int>("companyId");

            var statistics = new TaskStatistics();

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                statistics.CountTasksAsAuthor = await _unitOfWork.Task.GetAllTasksAsAuthorCount(companyId, user.Id);
                statistics.CountTasksAsCoExecutor = await _unitOfWork.Task.GetAllTasksAsCoExecutorCount(companyId, user.Id);
                statistics.CountTasksAsObserver = await _unitOfWork.Task.GetAllTasksAsObserverCount(companyId, user.Id);
                statistics.CountTasksAsResponsible = await _unitOfWork.Task.GetAllTasksAsResponsibleCount(companyId, user.Id);
                statistics.CountCompletedTasks = (await _unitOfWork.Task.GetAllCompletedTasks(companyId, user.Id)).Count;
                statistics.CountOverdueTasks = (await _unitOfWork.Task.GetAllOverdueTasks(companyId, user.Id)).Count;

                statistics.CountIssuedTasksPerMonth = await _unitOfWork.Task.GetCountIssuedTasksPerMonth(companyId);
                statistics.CountTasksCompletedPerMonth = await _unitOfWork.Task.GetCountTasksCompletedPerMonth(companyId);
                statistics.CountAllTasksCompletedPerMonth = await _unitOfWork.Task.GetCountAllTasksCompletedPerMonth(companyId);
                statistics.CountTasksOutstandingPerMonth = await _unitOfWork.Task.GetCountTasksOutstandingPerMonth(companyId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return statistics;
        }

        #endregion
    }
}