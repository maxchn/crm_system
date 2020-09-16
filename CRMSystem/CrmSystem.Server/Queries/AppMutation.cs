using CrmSystem.DAL;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.Extensions;
using CrmSystem.Server.GraphQL;
using CrmSystem.Server.Queries.InputTypes;
using CrmSystem.Server.Queries.Types;
using CrmSystem.Server.Services;
using CrmSystem.Server.Utils;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrmSystem.Server.Queries
{
    public class AppMutation : ObjectGraphType
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AppMutation(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailSender = emailSender;

            this.AuthorizeWith("Authorized");

            Field<ResultType>(
                name: "updateUser",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ApplicationUserInputType>> { Name = "user" },
                            new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: UpdateUser
            );

            Field<ResultType>(
                name: "updateCompany",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<CompanyInputType>> { Name = "company" }),
                resolve: UpdateCompany
            );

            Field<EmployeeRegisterResultType>(
                name: "createEmployee",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<EmployeeInputType>> { Name = "employee" },
                        new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" },
                        new QueryArgument<NonNullGraphType<BooleanGraphType>> { Name = "isSendLoginPasswordOnEmail" }),
                resolve: CreateNewEmployee
            );

            Field<ResultType>(
                name: "createTask",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<TaskInputType>> { Name = "task" },
                            new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" }),
                resolve: CreateTask
            );

            Field<ResultType>(
                name: "updateTask",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<TaskInputType>> { Name = "task" }),
                resolve: UpdateTask
            );

            Field<ResultType>(
                name: "removeTask",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" }),
                resolve: RemoveTask
            );

            Field<BooleanGraphType>(
                name: "reopenTask",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" }),
                resolve: ReopenTask
            );

            Field<ExecutionResultType>(
                name: "checkExecution",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>> { Name = "status" }),
                resolve: CheckExecution
            );

            Field<NonNullGraphType<BooleanGraphType>>(
                name: "markTaskCompletion",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" },
                                              new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: MarkTaskCompletion
            );

            Field<NonNullGraphType<ChatType>>(
                name: "createChat",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ChatInputType>> { Name = "chat" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "companyId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: CreateChat
            );

            Field<NonNullGraphType<TaskCommentType>>(
                name: "createTaskComment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "text" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taskId" },
                    new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "date" }),
                resolve: CreateTaskComment
            );

            Field<NonNullGraphType<ResultType>>(
                name: "deleteTaskComment",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: DeleteTaskComment
            );

            Field<NonNullGraphType<CalendarEventType>>(
                name: "createCalendarEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<CalendarEventInputType>> { Name = "calendarEvent" }),
                resolve: CreateCalendarEvent
            );

            Field<NonNullGraphType<CalendarEventType>>(
                name: "updateCalendarEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<CalendarEventInputType>> { Name = "calendarEvent" }),
                resolve: UpdateCalendarEvent
            );

            Field<NonNullGraphType<IntGraphType>>(
                name: "deleteCalendarEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "calendarEventId" }),
                resolve: DeleteCalendarEvent
            );

            Field<BooleanGraphType>(
                name: "changeDateTimeCalendarEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "calendarEventId" },
                                        new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "startDateTime" },
                                        new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "endDateTime" }),
                resolve: ChangeDateTimeCalendarEvent
            );

            Field<NonNullGraphType<ShortLinkType>>(
                name: "createShortLink",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ShortLinkInputType>> { Name = "shortLink" }),
                resolve: CreateShortLink
            );

            Field<NonNullGraphType<ResultType>>(
                name: "deleteShortLink",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "shortLinkId" }),
                resolve: DeleteShortLink
            );

            Field<NonNullGraphType<PrivateNotificationType>>(
                name: "createPrivateNotification",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<PrivateNotificationInputType>> { Name = "privateNotification" },
                    new QueryArgument<NonNullGraphType<ListGraphType<PrivateNotificationEmployeeInputType>>> { Name = "notificationEmployees" }),
                resolve: CreatePrivateNotification
            );

            Field<NonNullGraphType<PrivateNotificationType>>(
                name: "updatePrivateNotification",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<PrivateNotificationInputType>> { Name = "privateNotification" },
                    new QueryArgument<NonNullGraphType<ListGraphType<PrivateNotificationEmployeeInputType>>> { Name = "notificationEmployees" }),
                resolve: UpdatePrivateNotification
            );

            Field<NonNullGraphType<IntGraphType>>(
                name: "deletePrivateNotification",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: DeletePrivateNotification
            );
        }

        #region User(Employee) block

        private async Task<Result> UpdateUser(ResolveFieldContext<object> context)
        {
            var user = context.GetArgument<ApplicationUser>("user");
            var companyId = context.GetArgument<int>("companyId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                if (!user.Id.Equals(currentUser.Id))
                {
                    var company = await _unitOfWork.Company.FindById(companyId);

                    if (!currentUser.Id.Equals(company.Owner.Id))
                        throw new Exception("Отсутствуют права на данное действие!!!");
                }

                await _unitOfWork.User.Update(user);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return new Result { Status = true, Message = String.Empty };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new Result { Status = false, Message = ex.Message };
            }
        }

        private async Task<EmployeeRegisterResult> CreateNewEmployee(ResolveFieldContext<object> context)
        {
            var companyId = context.GetArgument<int>("companyId");
            var isSendLoginPasswordOnEmail = context.GetArgument<bool>("isSendLoginPasswordOnEmail");
            var newEmployee = context.GetArgument<ApplicationUser>("employee");
            var actionResult = new EmployeeRegisterResult();

            try
            {
                // Если был выбран пункт "Отправить логин и пароль на указанный e-mail", то 
                // тогда генерируем пароль, создаем пользователя и 
                // отправляем логин и пароль на указанный email
                if (isSendLoginPasswordOnEmail)
                {
                    string password = PasswordGenerator.Generate();

                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        // Ищем указанный отдел
                        var department = (await _unitOfWork.Department.Find(d =>
                            d.Name.ToLower().Equals(newEmployee.Department.Name.ToLower()))).FirstOrDefault();

                        // Если он найдена то добавляем его
                        if (department is null)
                        {
                            await _unitOfWork.Department.Insert(newEmployee.Department);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            newEmployee.Department = department;
                        }

                        // Ищем указанную должность
                        var position = (await _unitOfWork.Position.Find(p =>
                            p.Name.ToLower().Equals(newEmployee.Position.Name.ToLower()))).FirstOrDefault();

                        // Если она не найдена то добавляем ее
                        if (position is null)
                        {
                            await _unitOfWork.Position.Insert(newEmployee.Position);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            newEmployee.Position = position;
                        }

                        newEmployee.UserName = newEmployee.Email;

                        // Создаем нового пользователя
                        var result = await _userManager.CreateAsync(newEmployee, password);

                        if (result.Succeeded)
                        {
                            // Добавляем успешно созданого пользователя к указанной компании
                            await _unitOfWork.Company.AddEmployee(companyId, newEmployee);
                            await _unitOfWork.SaveChangesAsync();

                            // Отправляем на указанный e-mail письмо с инструкциями, логином и паролем
                            ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                            await _emailSender.SendRegestrationDateAsync(String.Format("{0} {1} {2}",
                                user.LastName,
                                user.FirstName,
                                user.Patronymic), newEmployee.Email, password, String.Format("{0}login", _configuration["WebClientUrl"]));

                            // Создаем уведомление о присоединении нового сотрудника к компании
                            CompanyNotification newCompanyNotification = new CompanyNotification
                            {
                                Author = user,
                                CompanyId = companyId,
                                NewEmployee = newEmployee,
                                Type = DAL.Entities.CompanyNotificationType.EmployeeJoin,
                                DateTime = DateTime.Now
                            };

                            await _unitOfWork.CompanyNotification.Insert(newCompanyNotification);
                        }
                        else
                        {
                            throw new Exception("Пользователь не был создан!!");
                        }

                        _unitOfWork.Commit();

                        actionResult.Status = true;
                        actionResult.Employee = newEmployee;
                    }
                    catch (Exception)
                    {
                        actionResult.Status = false;
                        actionResult.Employee = null;

                        _unitOfWork.Rollback();
                    }
                }
                else
                {
                    ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                    // Иначе создаем запрос на регистрацию на базе указанных даннных
                    string code = await _unitOfWork.User.CreateRegistrationRequest(newEmployee, user.Id);

                    if (!string.IsNullOrEmpty(code))
                    {
                        await _emailSender.SendInvitationAsync(newEmployee.Email, _configuration["WebClientUrl"] + $"join?ref={code}&email={newEmployee.Email}");
                        actionResult.Status = true;
                    }
                    else
                    {
                        actionResult.Status = false;
                        actionResult.Employee = null;
                    }
                }
            }
            catch (Exception)
            {
                actionResult.Status = false;
                actionResult.Employee = null;
            }

            return actionResult;
        }

        #endregion

        #region Company block

        private async Task<Result> UpdateCompany(ResolveFieldContext<object> context)
        {
            var company = context.GetArgument<Company>("company");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Company.Update(company);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return new Result { Status = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new Result { Status = false, Message = ex.Message };
            }
        }

        #endregion

        #region Task block

        private async Task<Result> CreateTask(ResolveFieldContext<object> context)
        {
            var task = context.GetArgument<MTask>("task");
            var companyId = context.GetArgument<int>("companyId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                AttachingFiles(task, user, task.Company.CompanyId);

                await _unitOfWork.Task.Insert(task);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return new Result { Status = true, Value = task.TaskId.ToString() };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new Result { Status = false, Message = ex.Message };
            }
        }

        private async Task<Result> UpdateTask(ResolveFieldContext<object> context)
        {
            var task = context.GetArgument<MTask>("task");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                AttachingFiles(task, user, task.Company.CompanyId);

                await _unitOfWork.Task.Update(task);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return new Result { Status = true };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new Result { Status = false, Message = ex.Message };
            }
        }

        private void AttachingFiles(MTask task, ApplicationUser user, int companyId)
        {
            string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, companyId.ToString());

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < task.AttachedFiles.Count; i++)
            {
                builder.Clear();

                if (task.AttachedFiles[i].AttachedFile != null &&
                    task.AttachedFiles[i].AttachedFile.AttachedFileId <= 0)
                {
                    builder.Append(basePath + task.AttachedFiles[i].AttachedFile.Path);
                    if (!System.IO.File.Exists(builder.ToString()))
                    {
                        task.AttachedFiles.Remove(task.AttachedFiles[i]);
                        i--;
                    }
                    else
                    {
                        task.AttachedFiles[i].AttachedFile.Owner = user;
                        task.AttachedFiles[i].AttachedFile.Name = Path.GetFileName(task.AttachedFiles[i].AttachedFile.Path);
                        task.AttachedFiles[i].AttachedFile.Path = builder.ToString().Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), "");
                    }
                }
            }
        }

        private async Task<Result> RemoveTask(ResolveFieldContext<object> context)
        {
            var taskId = context.GetArgument<int>("taskId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Task.Delete(taskId);

                var notifications = await _unitOfWork.TaskNotification.Find(n => n.Task.TaskId == taskId);

                foreach (var notification in notifications)
                {
                    await _unitOfWork.TaskNotification.Delete(notification.TaskNotificationId);
                }

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return new Result { Status = true };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new Result { Status = false, Message = ex.Message };
            }
        }

        private async Task<bool> ReopenTask(ResolveFieldContext<object> context)
        {
            var taskId = context.GetArgument<int>("taskId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {

                var task = await _unitOfWork.Task.FindById(taskId);
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                if (task.FinalPerformerId == null || !task.FinalPerformerId.Equals(user.Id))
                    throw new Exception("Данное действие запрещено!!!");

                task.FinalPerformerId = null;

                await _unitOfWork.Task.CreateTaskNotification(task, user, DAL.Entities.TaskNotificationType.Reopen, task.Company);

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<ExecutionResult> CheckExecution(ResolveFieldContext<object> context)
        {
            var taskId = context.GetArgument<int>("taskId");
            var status = context.GetArgument<bool>("status");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);
                var result = await _unitOfWork.Task.MarkExecution(taskId, status, user);
                _unitOfWork.Commit();
                result.Status = true;

                return result;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                return new ExecutionResult { Status = false };
            }
        }

        private async Task<bool> MarkTaskCompletion(ResolveFieldContext<object> context)
        {
            var taskId = context.GetArgument<int>("taskId");
            var userId = context.GetArgument<string>("userId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Task.MarkCompleted(taskId, userId);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {

                _unitOfWork.Rollback();
                return false;
            }
        }

        #endregion

        #region Chat block

        private async Task<Chat> CreateChat(ResolveFieldContext<object> context)
        {
            var chat = context.GetArgument<Chat>("chat");
            var companyId = context.GetArgument<int>("companyId");
            var userId = context.GetArgument<string>("userId");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                chat.Company = new Company
                {
                    CompanyId = companyId
                };

                chat.Owner = new ApplicationUser
                {
                    Id = userId
                };

                await _unitOfWork.Chat.Insert(chat);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return chat;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _unitOfWork.Rollback();
                return null;
            }
        }

        #endregion

        #region Chat comment block

        private async Task<TaskComment> CreateTaskComment(ResolveFieldContext<object> context)
        {
            var text = context.GetArgument<string>("text");
            var userId = context.GetArgument<string>("userId");
            var taskId = context.GetArgument<int>("taskId");
            var date = context.GetArgument<DateTime>("date");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var newComment = new TaskComment
                {
                    Text = text,
                    Date = date,
                    Task = new MTask { TaskId = taskId },
                    Author = new ApplicationUser { Id = userId }
                };

                await _unitOfWork.TaskComment.Insert(newComment);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return newComment;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _unitOfWork.Rollback();
                return null;
            }
        }

        private async Task<Result> DeleteTaskComment(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var taskComment = await _unitOfWork.TaskComment.FindById(id);

                if (taskComment != null)
                {
                    ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                    if (taskComment.AuthorId.Equals(user.Id))
                    {
                        await _unitOfWork.TaskComment.Delete(taskComment);
                        await _unitOfWork.SaveChangesAsync();

                        _unitOfWork.Commit();
                    }
                    else
                    {
                        throw new Exception("У Вас нет прав на удаление данного комментария!!!");
                    }
                }
                else
                {
                    throw new Exception("Коментарий по указаному идентификатору не найден!!!");
                }

                return new Result { Status = true, Value = id.ToString() };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);

                return new Result { Status = false, Message = ex.Message };
            }
        }

        #endregion

        #region Calendar Event block

        private async Task<CalendarEvent> CreateCalendarEvent(ResolveFieldContext<object> context)
        {
            var calendarEvent = context.GetArgument<CalendarEvent>("calendarEvent");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CalendarEvent.Insert(calendarEvent);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return calendarEvent;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<CalendarEvent> UpdateCalendarEvent(ResolveFieldContext<object> context)
        {
            var calendarEvent = context.GetArgument<CalendarEvent>("calendarEvent");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CalendarEvent.Update(calendarEvent);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return calendarEvent;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<int> DeleteCalendarEvent(ResolveFieldContext<object> context)
        {
            int calendarEventId = context.GetArgument<int>("calendarEventId");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.CalendarEvent.Delete(calendarEventId);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return calendarEventId;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        private async Task<bool> ChangeDateTimeCalendarEvent(ResolveFieldContext<object> context)
        {
            int calendarEventId = context.GetArgument<int>("calendarEventId");
            DateTime start = context.GetArgument<DateTime>("startDateTime");
            DateTime end = context.GetArgument<DateTime>("endDateTime");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var calendarEvent = await _unitOfWork.CalendarEvent.FindById(calendarEventId);
                calendarEvent.Start = start;
                calendarEvent.End = end;

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _unitOfWork.Rollback();
                return false;
            }
        }

        #endregion

        #region Short Link block

        private async Task<ShortLink> CreateShortLink(ResolveFieldContext<object> context)
        {
            var shortLink = context.GetArgument<ShortLink>("shortLink");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Проверяем что на указанный url не была создана короткая ссылка
                // и что это ранее созданная сокращенная ссылка
                var link = (await _unitOfWork.ShortLink.Find(l => l.Full.Equals(shortLink.Full) || l.Short.Equals(shortLink.Full)))?.FirstOrDefault();

                if (link is null)
                {
                    StringBuilder builder = new StringBuilder();
                    int countShortLink = -1;

                    do
                    {
                        // Генерируем сокращенную ссылку
                        builder.Clear();
                        builder.Append(LinkGenerator.GenerateShortLink($"{_configuration["BaseDomain"]}u/"));

                        // Проверяем что сгенерированной сокращенной ссылки еще не в БД
                        countShortLink = (await _unitOfWork.ShortLink.Find(l => l.Short.Equals(builder.ToString()))).Count();
                    }
                    while (countShortLink != 0);

                    shortLink.Short = builder.ToString();

                    await _unitOfWork.ShortLink.Insert(shortLink);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    return shortLink;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<Result> DeleteShortLink(ResolveFieldContext<object> context)
        {
            var shortLinkId = context.GetArgument<int>("shortLinkId");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Находим сокращенную ссылку
                var shortLink = await _unitOfWork.ShortLink.FindById(shortLinkId);

                // Ищем ссылается сокращенная ссылка на краткую ссылку
                var filePublicLink = await _unitOfWork.FilePublicLink.Find(p => shortLink.Full.Equals($"{_configuration["BaseDomain"]}share?code={p.Link}"));

                // Если переменная filePublicLink не равно null значит 
                // сокращенная ссылка ссылается на публичную ссылку файла
                // то такую ссылку нельзя удалять
                if (filePublicLink != null && filePublicLink.Count() > 0)
                    throw new Exception("Сокращенная ссылка ссылается на публичную ссылку файла. Сначала удалите пуличную ссылку на файл, затем удаляйте сокращенную ссылку!!!");

                await _unitOfWork.ShortLink.Delete(shortLinkId);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return new Result { Status = true, Value = shortLinkId.ToString() };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Debug.WriteLine(ex.Message);
                return new Result { Status = false, Message = ex.Message };
            }
        }

        #endregion

        #region Notification block

        private async Task<PrivateNotification> CreatePrivateNotification(ResolveFieldContext<object> context)
        {
            var privateNotification = context.GetArgument<PrivateNotification>("privateNotification");
            var notificationEmployees = context.GetArgument<List<PrivateNotificationEmployee>>("notificationEmployees");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                privateNotification.DateTime = DateTime.Now;
                await _unitOfWork.PrivateNotification.Insert(privateNotification);
                await _unitOfWork.SaveChangesAsync();

                HashSet<string> users = new HashSet<string>();

                notificationEmployees.ForEach(async i =>
                {
                    if (!users.Contains(i.EmployeeId))
                    {
                        i.PrivateNotificationId = privateNotification.PrivateNotificationId;
                        await _unitOfWork.PrivateNotificationEmployee.Insert(i);

                        users.Add(i.EmployeeId);
                    }
                });

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return privateNotification;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _unitOfWork.Rollback();
                return null;
            }
        }

        private async Task<PrivateNotification> UpdatePrivateNotification(ResolveFieldContext<object> context)
        {
            var updatedPrivateNotification = context.GetArgument<PrivateNotification>("privateNotification");
            var updatedNotificationEmployees = context.GetArgument<List<PrivateNotificationEmployee>>("notificationEmployees");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var oldNotificationEmployees = await _unitOfWork.PrivateNotificationEmployee.Find(n => n.PrivateNotificationId == updatedPrivateNotification.PrivateNotificationId);

                foreach (var item in oldNotificationEmployees)
                {
                    await _unitOfWork.PrivateNotificationEmployee.Delete(item.Id);
                }

                var privateNotification = await _unitOfWork.PrivateNotification.FindById(updatedPrivateNotification.PrivateNotificationId);

                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                if (!privateNotification.AuthorId.Equals(user.Id))
                    throw new Exception("Нет прав на редактирование!!!");

                privateNotification.DateTime = DateTime.Now;
                privateNotification.Body = updatedPrivateNotification.Body;

                await _unitOfWork.SaveChangesAsync();

                HashSet<string> users = new HashSet<string>();

                updatedNotificationEmployees.ForEach(async i =>
                {
                    if (!users.Contains(i.EmployeeId))
                    {
                        i.PrivateNotificationId = privateNotification.PrivateNotificationId;
                        await _unitOfWork.PrivateNotificationEmployee.Insert(i);

                        users.Add(i.EmployeeId);
                    }
                });

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return privateNotification;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _unitOfWork.Rollback();
                return null;
            }
        }

        private async Task<int> DeletePrivateNotification(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<int>("id");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var notificationEmployees = await _unitOfWork.PrivateNotificationEmployee.Find(n => n.PrivateNotificationId == id);

                foreach (var item in notificationEmployees)
                {
                    await _unitOfWork.PrivateNotificationEmployee.Delete(item.Id);
                }

                var notification = await _unitOfWork.PrivateNotification.FindById(id);

                ApplicationUser user = await _userManager.GetUserAsync((ClaimsPrincipal)context.UserContext);

                if (!notification.AuthorId.Equals(user.Id))
                    throw new Exception("Нет прав на удаление!!!");

                await _unitOfWork.PrivateNotification.Delete(id);

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _unitOfWork.Rollback();
                return 0;
            }
        }

        #endregion
    }
}