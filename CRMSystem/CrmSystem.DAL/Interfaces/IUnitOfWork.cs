using CrmSystem.DAL.Repositories;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Interfaces
{
    /// <summary>
    /// Менеджер репозиториев
    /// </summary>
    public interface IUnitOfWork
    {
        NpsqlApplicationUserRepository User { get; }
        NpsqlCompanyRepository Company { get; }
        NpsqlPositionRepository Position { get; }
        NpsqlDepartmentRepository Department { get; }
        NpsqlTaskRepository Task { get; }
        NpsqlChatRepository Chat { get; }
        NpsqlChatMessageRepository ChatMessage { get; }
        NpsqlChatMessageParticipantRepository ChatMessageParticipant { get; }
        NpsqlTaskCommentRepository TaskComment { get; }
        NpsqlFilePublickLinkRepository FilePublicLink { get; }
        NpsqlShortLinkRepository ShortLink { get; }
        NpsqlCalendarEventRepository CalendarEvent { get; }
        NpsqlAttachedFileRepository AttachedFile { get; }
        NpsqlTaskAttachedFileRepository TaskAttachedFile { get; }
        NpsqlCompanyNotificationRepository CompanyNotification { get; }
        NpsqlRegistrationRequestRepository RegistrationRequest { get; }
        NpsqlEmployeeInvitationRepository EmployeeInvitation { get; }
        NpsqlTaskNotificationEmployeeRepository TaskNotificationEmployee { get; }
        NpsqlTaskNotificationRepository TaskNotification { get; }
        NpsqlPrivateNotificationRepository PrivateNotification { get; }
        NpsqlPrivateNotificationEmployeeRepository PrivateNotificationEmployee { get; }

        /// <summary>
        /// Создание транзакции
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Коммит (подтверждение) транзакции
        /// </summary>
        void Commit();

        /// <summary>
        /// Отмена транзакции
        /// </summary>
        void Rollback();

        /// <summary>
        /// Сохранение текущих изменений
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();

        void DetachEntity(object entity);

        void EnabledDisabledAutoDetectChanges(bool enable);
    }
}