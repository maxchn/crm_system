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
    public class NpsqlPrivateNotificationEmployeeRepository : IGenericRepository<PrivateNotificationEmployee>
    {
        private NpgsqlDbContext _context;

        public NpsqlPrivateNotificationEmployeeRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var notificationEmployee = await FindById(id);

            if (notificationEmployee is null)
                throw new ArgumentException("Private Notification Employee with the specified ID not found!!!");

            _context.PrivateNotificationEmployees.Remove(notificationEmployee);
        }

        public async Task<IEnumerable<PrivateNotificationEmployee>> Find(Expression<Func<PrivateNotificationEmployee, bool>> predicate)
        {
            return await _context.PrivateNotificationEmployees
                .Include(c => c.PrivateNotification)
                .Include(c => c.Employee)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrivateNotificationEmployee>> FindAll()
        {
            return await _context.PrivateNotificationEmployees
                .Include(c => c.PrivateNotification)
                .Include(c => c.Employee)
                .ToListAsync();
        }

        public async Task<PrivateNotificationEmployee> FindById(object id)
        {
            return await _context.PrivateNotificationEmployees
                .Include(c => c.PrivateNotification)
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(x => x.Id == (int)id);
        }

        public async Task Insert(PrivateNotificationEmployee entity)
        {
            await _context.PrivateNotificationEmployees.AddAsync(entity);
        }

        public Task Update(PrivateNotificationEmployee entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}