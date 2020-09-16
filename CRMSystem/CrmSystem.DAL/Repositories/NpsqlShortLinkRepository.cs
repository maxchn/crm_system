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
    public class NpsqlShortLinkRepository : IGenericRepository<ShortLink>
    {
        private NpgsqlDbContext _context;

        public NpsqlShortLinkRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var link = await FindById(id);

            if (link is null)
                throw new ArgumentException("Link with the specified ID not found!!!");

            _context.ShortLinks.Remove(link);
        }

        public async Task<IEnumerable<ShortLink>> Find(Expression<Func<ShortLink, bool>> predicate)
        {
            return await _context.ShortLinks                
                .Where(predicate)
                .ToListAsync();
        }

        public Task<IEnumerable<ShortLink>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ShortLink> FindById(object id)
        {
            return await _context.ShortLinks.FirstOrDefaultAsync(l => l.ShortLinkId == (int)id);
        }

        public async Task Insert(ShortLink entity)
        {
            await _context.ShortLinks.AddAsync(entity);
        }

        public Task Update(ShortLink entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}