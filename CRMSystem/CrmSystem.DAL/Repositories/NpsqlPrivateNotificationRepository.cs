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
    public class NpsqlPrivateNotificationRepository : IGenericRepository<PrivateNotification>
    {
        private NpgsqlDbContext _context;

        public NpsqlPrivateNotificationRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var notification = await FindById(id);

            if (notification is null)
                throw new ArgumentException("Private Notification with the specified ID not found!!!");

            _context.PrivateNotifications.Remove(notification);
        }

        public async Task<IEnumerable<PrivateNotification>> Find(Expression<Func<PrivateNotification, bool>> predicate)
        {
            return await _context.PrivateNotifications
                .Include(c => c.Author)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrivateNotification>> FindAll()
        {
            return await _context.PrivateNotifications
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<PrivateNotification> FindById(object id)
        {
            return await _context.PrivateNotifications
                .Include(c => c.Author)
                .FirstOrDefaultAsync(x => x.PrivateNotificationId == (int)id);
        }

        public async Task Insert(PrivateNotification entity)
        {
            await _context.PrivateNotifications.AddAsync(entity);
        }

        public Task Update(PrivateNotification entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}