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
    public class NpsqlFilePublickLinkRepository : IGenericRepository<FilePublicLink>
    {
        private NpgsqlDbContext _context;

        public NpsqlFilePublickLinkRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var link = await FindById(id);

            if (link is null)
                throw new ArgumentException("Link with the specified ID not found!!!");

            _context.FilePublicLinks.Remove(link);
        }

        public async Task<IEnumerable<FilePublicLink>> Find(Expression<Func<FilePublicLink, bool>> predicate)
        {
            return await _context.FilePublicLinks
                .Include(l => l.Owner)
                .Include(l => l.ShortLink)
                .Where(predicate)
                .ToListAsync();
        }

        public Task<IEnumerable<FilePublicLink>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<FilePublicLink> FindById(object id)
        {
            return await _context.FilePublicLinks.FirstOrDefaultAsync(l => l.FilePublicLinkId == (int)id);
        }

        public async Task Insert(FilePublicLink entity)
        {
            await _context.FilePublicLinks.AddAsync(entity);
        }

        public Task Update(FilePublicLink entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}