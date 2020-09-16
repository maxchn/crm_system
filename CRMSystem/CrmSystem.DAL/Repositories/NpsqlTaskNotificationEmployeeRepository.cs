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
    public class NpsqlTaskNotificationEmployeeRepository : IGenericRepository<TaskNotificationEmployee>
    {
        private NpgsqlDbContext _context;

        public NpsqlTaskNotificationEmployeeRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var entity = await FindById(id);

            if (entity is null)
                throw new ArgumentException("NotificationEmployee with the specified ID not found!!!");

            _context.TaskNotificationEmployees.Remove(entity);
        }

        public async Task<IEnumerable<TaskNotificationEmployee>> Find(Expression<Func<TaskNotificationEmployee, bool>> predicate)
        {
            return await _context.TaskNotificationEmployees
                .Include(c => c.Employee)
                .Include(c => c.TaskNotification)
                .Include(c => c.Company)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskNotificationEmployee>> FindAll()
        {
            return await _context.TaskNotificationEmployees
                .Include(c => c.Employee)
                .Include(c => c.TaskNotification)
                .Include(c => c.Company)
                .ToListAsync();
        }

        public async Task<TaskNotificationEmployee> FindById(object id)
        {
            return await _context.TaskNotificationEmployees
                .Include(c => c.Employee)
                .Include(c => c.TaskNotification)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(x => x.TaskNotificationEmployeeId == (int)id);
        }

        public async Task Insert(TaskNotificationEmployee entity)
        {
            await _context.TaskNotificationEmployees.AddAsync(entity);
        }

        public Task Update(TaskNotificationEmployee entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}