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
    public class NpsqlAttachedFileRepository : IGenericRepository<AttachedFile>
    {
        private NpgsqlDbContext _context;

        public NpsqlAttachedFileRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var attachedFile = await FindById(id);

            if (attachedFile is null)
                throw new ArgumentException("Attached File with the specified ID not found!!!");

            _context.AttachedFiles.Remove(attachedFile);
        }

        public async Task<IEnumerable<AttachedFile>> Find(Expression<Func<AttachedFile, bool>> predicate)
        {
            return await _context.AttachedFiles
                .Include(c => c.Owner)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AttachedFile>> FindAll()
        {
            return await _context.AttachedFiles
                .Include(c => c.Owner)
                .ToListAsync();
        }

        public async Task<AttachedFile> FindById(object id)
        {
            return await _context.AttachedFiles
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(x => x.AttachedFileId == (int)id);
        }

        public async Task Insert(AttachedFile entity)
        {
            await _context.AttachedFiles.AddAsync(entity);
        }

        public Task Update(AttachedFile entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
