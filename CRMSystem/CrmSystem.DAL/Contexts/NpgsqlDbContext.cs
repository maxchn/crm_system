using CrmSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CrmSystem.DAL.Contexts
{
    public class NpgsqlDbContext : IdentityDbContext<ApplicationUser>
    {
        public NpgsqlDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();

            if (!Genders.Any())
            {
                var male = new Gender { Name = "Мужской" };
                var female = new Gender { Name = "Женский" };
                var other = new Gender { Name = "Другой" };

                Genders.AddRange(new[] { male, female, other });
                SaveChanges();
            }
        }

        public DbSet<CoExecutor> CoExecutors { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyDepartment> CompanyDepartments { get; set; }
        public DbSet<CompanyEmployee> CompanyEmployees { get; set; }
        public DbSet<CompanyPosition> CompanyPositions { get; set; }
        public DbSet<CompanyTag> CompanyTags { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<MTask> Tasks { get; set; }
        public DbSet<Observer> Observers { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<ResponsibleForExecution> ResponsibleForExecutions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatMessageParticipant> ChatMessageParticipants { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<FilePublicLink> FilePublicLinks { get; set; }
        public DbSet<ShortLink> ShortLinks { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<TaskAttachedFile> TaskAttachedFiles { get; set; }
        public DbSet<TaskNotification> TaskNotifications { get; set; }
        public DbSet<TaskNotificationEmployee> TaskNotificationEmployees { get; set; }
        public DbSet<CompanyNotification> CompanyNotifications { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public DbSet<EmployeeInvitation> EmployeeInvitations { get; set; }
        public DbSet<PrivateNotification> PrivateNotifications { get; set; }
        public DbSet<PrivateNotificationEmployee> PrivateNotificationEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            builder.Entity<Position>()
                .HasIndex(p => p.Name)
                .IsUnique();

            builder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();

            builder.Entity<Gender>()
                .HasIndex(g => g.Name)
                .IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // The many-to-many relationship between the "Task" and the "ApplicationUser"
            builder.Entity<CoExecutor>()
                .HasKey(ce => new { ce.TaskId, ce.Id });

            builder.Entity<CoExecutor>()
                .HasOne(ce => ce.Task)
                .WithMany(t => t.CoExecutors)
                .HasForeignKey(ce => ce.TaskId);

            builder.Entity<CoExecutor>()
                .HasOne(ce => ce.User)
                .WithMany(t => t.CoExecutors)
                .HasForeignKey(ce => ce.Id);

            // The many-to-many relationship between the "Company" and the "Department"
            builder.Entity<CompanyDepartment>()
                .HasKey(cd => new { cd.CompanyId, cd.DepartmentId });

            builder.Entity<CompanyDepartment>()
                .HasOne(cd => cd.Company)
                .WithMany(d => d.CompanyDepartments)
                .HasForeignKey(cd => cd.CompanyId);

            builder.Entity<CompanyDepartment>()
                .HasOne(cd => cd.Department)
                .WithMany(d => d.CompanyDepartments)
                .HasForeignKey(cd => cd.DepartmentId);

            // The many-to-many relationship between the "Company" and the "Position"
            builder.Entity<CompanyPosition>()
                .HasKey(cp => new { cp.CompanyId, cp.PositionId });

            builder.Entity<CompanyPosition>()
                .HasOne(cp => cp.Company)
                .WithMany(d => d.CompanyPositions)
                .HasForeignKey(cp => cp.CompanyId);

            builder.Entity<CompanyPosition>()
                .HasOne(cp => cp.Position)
                .WithMany(d => d.CompanyPositions)
                .HasForeignKey(cp => cp.PositionId);

            // The many-to-many relationship between the "Company" and the "Tag"
            builder.Entity<CompanyTag>()
                .HasKey(ct => new { ct.CompanyId, ct.TagId });

            builder.Entity<CompanyTag>()
                .HasOne(ct => ct.Company)
                .WithMany(d => d.CompanyTags)
                .HasForeignKey(ct => ct.CompanyId);

            builder.Entity<CompanyTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(d => d.CompanyTags)
                .HasForeignKey(ct => ct.TagId);

            // The many-to-many relationship between the "Task" and the "ApplicationUser"
            builder.Entity<Observer>()
                .HasKey(o => new { o.TaskId, o.Id });

            builder.Entity<Observer>()
                .HasOne(o => o.Task)
                .WithMany(t => t.Observers)
                .HasForeignKey(o => o.TaskId);

            builder.Entity<Observer>()
                .HasOne(o => o.User)
                .WithMany(t => t.Observers)
                .HasForeignKey(o => o.Id);

            // The many-to-many relationship between the "Task" and the "ApplicationUser"
            builder.Entity<ResponsibleForExecution>()
                .HasKey(r => new { r.TaskId, r.Id });

            builder.Entity<ResponsibleForExecution>()
                .HasOne(r => r.Task)
                .WithMany(t => t.ResponsiblesForExecution)
                .HasForeignKey(r => r.TaskId);

            builder.Entity<ResponsibleForExecution>()
                .HasOne(r => r.User)
                .WithMany(t => t.ResponsiblesForExecution)
                .HasForeignKey(r => r.Id);

            // The many-to-many relationship between the "Tag" and the "Task"
            builder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskId, tt.TagId });

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId);

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId);

            // The many-to-many relationship between the "Chat" and the "ApplicationUser"
            builder.Entity<ChatParticipant>()
                .HasKey(cp => new { cp.ChatId, cp.Id });

            builder.Entity<ChatParticipant>()
                .HasOne(cp => cp.Chat)
                .WithMany(t => t.ChatParticipants)
                .HasForeignKey(cp => cp.ChatId);

            builder.Entity<ChatParticipant>()
                .HasOne(cp => cp.User)
                .WithMany(t => t.ChatParticipants)
                .HasForeignKey(cp => cp.Id);
        }
    }
}