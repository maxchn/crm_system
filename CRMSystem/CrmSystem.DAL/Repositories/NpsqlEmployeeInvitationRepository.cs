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
    public class NpsqlEmployeeInvitationRepository : IGenericRepository<EmployeeInvitation>
    {
        private NpgsqlDbContext _context;

        public NpsqlEmployeeInvitationRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var invitation = await FindById(id);

            if (invitation is null)
                throw new ArgumentException("Employee Invitation with the specified ID not found!!!");

            _context.EmployeeInvitations.Remove(invitation);
        }

        public async Task<IEnumerable<EmployeeInvitation>> Find(Expression<Func<EmployeeInvitation, bool>> predicate)
        {
            return await _context.EmployeeInvitations
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInvitation>> FindAll()
        {
            return await _context.EmployeeInvitations.ToListAsync();
        }

        public async Task<EmployeeInvitation> FindById(object id)
        {
            return await _context.EmployeeInvitations
                .FirstOrDefaultAsync(x => x.EmployeeInvitationId == (int)id);
        }

        public async Task Insert(EmployeeInvitation entity)
        {
            await _context.EmployeeInvitations.AddAsync(entity);
        }

        public Task Update(EmployeeInvitation entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}