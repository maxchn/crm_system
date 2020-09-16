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
    public class NpsqlCompanyNotificationRepository : IGenericRepository<CompanyNotification>
    {
        private NpgsqlDbContext _context;

        public NpsqlCompanyNotificationRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var notification = await FindById(id);

            if (notification is null)
                throw new ArgumentException("Company Notification with the specified ID not found!!!");

            _context.CompanyNotifications.Remove(notification);
        }

        public async Task<IEnumerable<CompanyNotification>> Find(Expression<Func<CompanyNotification, bool>> predicate)
        {
            return await _context.CompanyNotifications
                .Include(c => c.Author)
                .Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<CompanyNotification>> FindAll()
        {
            return await _context.CompanyNotifications
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<CompanyNotification> FindById(object id)
        {
            return await _context.CompanyNotifications
                .Include(c => c.Author)
                .FirstOrDefaultAsync(x => x.CompanyNotificationId == (int)id);
        }

        public async Task Insert(CompanyNotification entity)
        {
            await _context.CompanyNotifications.AddAsync(entity);
        }

        public Task Update(CompanyNotification entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}