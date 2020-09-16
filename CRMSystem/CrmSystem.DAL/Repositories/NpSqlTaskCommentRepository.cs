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
    public class NpsqlTaskCommentRepository : IGenericRepository<TaskComment>
    {
        private NpgsqlDbContext _context;

        public NpsqlTaskCommentRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            //var comment = await FindById(id);

            //if (comment is null)
            //throw new ArgumentException("Comment with the specified ID not found!!!");

            _context.TaskComments.Remove(id as TaskComment);
        }

        public async Task<IEnumerable<TaskComment>> Find(Expression<Func<TaskComment, bool>> predicate)
        {
            return await _context.TaskComments.Include(x => x.Author)
                .Include(x => x.Task)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskComment>> FindAll()
        {
            return await _context.TaskComments.Include(x => x.Author)
                .Include(x => x.Task)
                .ToListAsync();
        }

        public async Task<TaskComment> FindById(object id)
        {
            return await _context.TaskComments.Include(x => x.Author)
                .Include(x => x.Task)
                .FirstOrDefaultAsync(x => x.TaskCommentId == (int)id);
        }

        public async Task Insert(TaskComment entity)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == entity.Task.TaskId);
            entity.Task = task;

            var author = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(entity.Author.Id));
            entity.Author = author;
            entity.IsAccessOnDeleting = true;

            await _context.TaskComments.AddAsync(entity);
        }

        public Task Update(TaskComment entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}