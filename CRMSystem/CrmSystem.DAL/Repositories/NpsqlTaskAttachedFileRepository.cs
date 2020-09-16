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
    public class NpsqlTaskAttachedFileRepository : IGenericRepository<TaskAttachedFile>
    {
        private NpgsqlDbContext _context;

        public NpsqlTaskAttachedFileRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var taskAttachedFile = await FindById(id);

            if (taskAttachedFile is null)
                throw new ArgumentException("Task Attached File with the specified ID not found!!!");

            _context.TaskAttachedFiles.Remove(taskAttachedFile);
        }

        public async Task<IEnumerable<TaskAttachedFile>> Find(Expression<Func<TaskAttachedFile, bool>> predicate)
        {
            return await _context.TaskAttachedFiles
                .Include(c => c.AttachedFile)
                .Include(c => c.Task)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskAttachedFile>> FindAll()
        {
            return await _context.TaskAttachedFiles
                .Include(c => c.AttachedFile)
                .Include(c => c.Task)
                .ToListAsync();
        }

        public async Task<TaskAttachedFile> FindById(object id)
        {
            return await _context.TaskAttachedFiles
                .Include(c => c.AttachedFile)
                .Include(c => c.Task)
                .FirstOrDefaultAsync(x => x.TaskAttachedFileId == (int)id);
        }

        public async Task Insert(TaskAttachedFile entity)
        {
            await _context.TaskAttachedFiles.AddAsync(entity);
        }

        public Task Update(TaskAttachedFile entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}