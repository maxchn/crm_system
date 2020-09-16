using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Interfaces;
using CrmSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace CrmSystem.DAL.UnitOfWork
{
    public class NpsqlUnitOfWork : IUnitOfWork
    {
        private NpgsqlDbContext _context;
        private IDbContextTransaction _transaction;

        private NpsqlApplicationUserRepository _userRepository;
        private NpsqlCompanyRepository _companyRepository;
        private NpsqlPositionRepository _positionRepository;
        private NpsqlDepartmentRepository _departmentRepository;
        private NpsqlTaskRepository _taskRepository;
        private NpsqlChatRepository _chatRepository;
        private NpsqlChatMessageRepository _chatMessageRepository;
        private NpsqlChatMessageParticipantRepository _chatParticipantRepository;
        private NpsqlTaskCommentRepository _taskCommentRepository;
        private NpsqlFilePublickLinkRepository _filePublicLinkRepository;
        private NpsqlShortLinkRepository _shortLinkRepository;
        private NpsqlCalendarEventRepository _calendarEventRepository;
        private NpsqlAttachedFileRepository _attachedFileRepository;
        private NpsqlTaskAttachedFileRepository _taskAttachedFileRepository;
        private NpsqlCompanyNotificationRepository _companyNotificationRepository;
        private NpsqlRegistrationRequestRepository _registrationRequestRepository;
        private NpsqlEmployeeInvitationRepository _employeeInvitationRepository;
        private NpsqlTaskNotificationEmployeeRepository _taskNotificationEmployeeRepository;
        private NpsqlTaskNotificationRepository _taskNotificationRepository;
        private NpsqlPrivateNotificationRepository _privateNotificationRepository;
        private NpsqlPrivateNotificationEmployeeRepository _privateNotificationEmployeeRepository;

        public NpsqlUnitOfWork(NpgsqlDbContext context)
        {
            _context = context;
        }

        public NpsqlApplicationUserRepository User
        {
            get
            {
                if (_userRepository is null)
                    _userRepository = new NpsqlApplicationUserRepository(_context);

                return _userRepository;
            }
        }

        public NpsqlCompanyRepository Company
        {
            get
            {
                if (_companyRepository is null)
                    _companyRepository = new NpsqlCompanyRepository(_context);

                return _companyRepository;
            }
        }

        public NpsqlPositionRepository Position
        {
            get
            {
                if (_positionRepository is null)
                    _positionRepository = new NpsqlPositionRepository(_context);

                return _positionRepository;
            }
        }

        public NpsqlDepartmentRepository Department
        {
            get
            {
                if (_departmentRepository is null)
                    _departmentRepository = new NpsqlDepartmentRepository(_context);

                return _departmentRepository;
            }
        }

        public NpsqlTaskRepository Task
        {
            get
            {
                if (_taskRepository is null)
                    _taskRepository = new NpsqlTaskRepository(_context);

                return _taskRepository;
            }
        }

        public NpsqlChatRepository Chat
        {
            get
            {
                if (_chatRepository is null)
                    _chatRepository = new NpsqlChatRepository(_context);

                return _chatRepository;
            }
        }

        public NpsqlChatMessageRepository ChatMessage
        {
            get
            {
                if (_chatMessageRepository is null)
                    _chatMessageRepository = new NpsqlChatMessageRepository(_context);

                return _chatMessageRepository;
            }
        }

        public NpsqlChatMessageParticipantRepository ChatMessageParticipant
        {
            get
            {
                if (_chatParticipantRepository is null)
                    _chatParticipantRepository = new NpsqlChatMessageParticipantRepository(_context);

                return _chatParticipantRepository;
            }
        }

        public NpsqlTaskCommentRepository TaskComment
        {
            get
            {
                if (_taskCommentRepository is null)
                    _taskCommentRepository = new NpsqlTaskCommentRepository(_context);

                return _taskCommentRepository;
            }
        }

        public NpsqlFilePublickLinkRepository FilePublicLink
        {
            get
            {
                if (_filePublicLinkRepository is null)
                    _filePublicLinkRepository = new NpsqlFilePublickLinkRepository(_context);

                return _filePublicLinkRepository;
            }
        }

        public NpsqlShortLinkRepository ShortLink
        {
            get
            {
                if (_shortLinkRepository is null)
                    _shortLinkRepository = new NpsqlShortLinkRepository(_context);

                return _shortLinkRepository;
            }
        }

        public NpsqlCalendarEventRepository CalendarEvent
        {
            get
            {
                if (_calendarEventRepository is null)
                    _calendarEventRepository = new NpsqlCalendarEventRepository(_context);

                return _calendarEventRepository;
            }
        }

        public NpsqlAttachedFileRepository AttachedFile
        {
            get
            {
                if (_attachedFileRepository is null)
                    _attachedFileRepository = new NpsqlAttachedFileRepository(_context);

                return _attachedFileRepository;
            }
        }

        public NpsqlTaskAttachedFileRepository TaskAttachedFile
        {
            get
            {
                if (_taskAttachedFileRepository is null)
                    _taskAttachedFileRepository = new NpsqlTaskAttachedFileRepository(_context);

                return _taskAttachedFileRepository;
            }
        }

        public NpsqlCompanyNotificationRepository CompanyNotification
        {
            get
            {
                if (_companyNotificationRepository is null)
                    _companyNotificationRepository = new NpsqlCompanyNotificationRepository(_context);

                return _companyNotificationRepository;
            }
        }

        public NpsqlRegistrationRequestRepository RegistrationRequest
        {
            get
            {
                if (_registrationRequestRepository is null)
                    _registrationRequestRepository = new NpsqlRegistrationRequestRepository(_context);

                return _registrationRequestRepository;
            }
        }

        public NpsqlEmployeeInvitationRepository EmployeeInvitation
        {
            get
            {
                if (_employeeInvitationRepository is null)
                    _employeeInvitationRepository = new NpsqlEmployeeInvitationRepository(_context);

                return _employeeInvitationRepository;
            }
        }

        public NpsqlTaskNotificationEmployeeRepository TaskNotificationEmployee
        {
            get
            {
                if (_taskNotificationEmployeeRepository is null)
                    _taskNotificationEmployeeRepository = new NpsqlTaskNotificationEmployeeRepository(_context);

                return _taskNotificationEmployeeRepository;
            }
        }

        public NpsqlTaskNotificationRepository TaskNotification
        {
            get
            {
                if (_taskNotificationRepository is null)
                    _taskNotificationRepository = new NpsqlTaskNotificationRepository(_context);

                return _taskNotificationRepository;
            }
        }

        public NpsqlPrivateNotificationRepository PrivateNotification
        {
            get
            {
                if (_privateNotificationRepository is null)
                    _privateNotificationRepository = new NpsqlPrivateNotificationRepository(_context);

                return _privateNotificationRepository;
            }
        }

        public NpsqlPrivateNotificationEmployeeRepository PrivateNotificationEmployee
        {
            get
            {
                if (_privateNotificationEmployeeRepository is null)
                    _privateNotificationEmployeeRepository = new NpsqlPrivateNotificationEmployeeRepository(_context);

                return _privateNotificationEmployeeRepository;
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void DetachEntity(object entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public void EnabledDisabledAutoDetectChanges(bool enable)
        {
            _context.ChangeTracker.QueryTrackingBehavior = enable ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}