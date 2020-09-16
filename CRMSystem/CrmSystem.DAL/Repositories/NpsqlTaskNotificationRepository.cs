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
    public class NpsqlTaskNotificationRepository : IGenericRepository<TaskNotification>
    {
        private NpgsqlDbContext _context;

        public NpsqlTaskNotificationRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var notification = await FindById(id);

            if (notification is null)
                throw new ArgumentException("Notification with the specified ID not found!!!");

            _context.TaskNotifications.Remove(notification);
        }

        public async Task<IEnumerable<TaskNotification>> Find(Expression<Func<TaskNotification, bool>> predicate)
        {
            return await _context.TaskNotifications
                .Include(c => c.Author)
                .Include(c => c.Task)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskNotification>> FindAll()
        {
            return await _context.TaskNotifications
                .Include(c => c.Author)
                .Include(c => c.Task)
                .ToListAsync();
        }

        public async Task<TaskNotification> FindById(object id)
        {
            return await _context.TaskNotifications
                .Include(c => c.Author)
                .Include(c => c.Task)
                .FirstOrDefaultAsync(x => x.TaskNotificationId == (int)id);
        }

        public async Task Insert(TaskNotification entity)
        {
            await _context.TaskNotifications.AddAsync(entity);
        }

        public Task Update(TaskNotification entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}