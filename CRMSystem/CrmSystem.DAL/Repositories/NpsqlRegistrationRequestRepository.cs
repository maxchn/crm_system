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
    public class NpsqlRegistrationRequestRepository : IGenericRepository<RegistrationRequest>
    {
        private NpgsqlDbContext _context;

        public NpsqlRegistrationRequestRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var request = await FindById(id);

            if (request is null)
                throw new ArgumentException("Registration Request with the specified ID not found!!!");

            _context.RegistrationRequests.Remove(request);
        }

        public async Task<IEnumerable<RegistrationRequest>> Find(Expression<Func<RegistrationRequest, bool>> predicate)
        {
            return await _context.RegistrationRequests
                .Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<RegistrationRequest>> FindAll()
        {
            return await _context.RegistrationRequests
                .ToListAsync();
        }

        public async Task<RegistrationRequest> FindById(object id)
        {
            return await _context.RegistrationRequests
                .FirstOrDefaultAsync(x => x.RegistrationRequestId == (int)id);
        }

        public async Task Insert(RegistrationRequest entity)
        {
            await _context.RegistrationRequests.AddAsync(entity);
        }

        public Task Update(RegistrationRequest entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}