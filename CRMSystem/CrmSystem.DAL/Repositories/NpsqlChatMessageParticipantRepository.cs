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
    public class NpsqlChatMessageParticipantRepository : IGenericRepository<ChatMessageParticipant>
    {
        private NpgsqlDbContext _context;

        public NpsqlChatMessageParticipantRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var entity = await FindById(id);

            if (entity is null)
                throw new ArgumentException("ChatMessageParticipant with the specified ID not found!!!");

            _context.ChatMessageParticipants.Remove(entity);
        }

        public async Task<IEnumerable<ChatMessageParticipant>> Find(Expression<Func<ChatMessageParticipant, bool>> predicate)
        {
            return await _context.ChatMessageParticipants
                .Include(x => x.Chat)
                .Include(x => x.Message)
                .Include(x => x.Message.Owner)
                .Include(x => x.User)
                .Where(predicate).ToListAsync();
        }

        public Task<IEnumerable<ChatMessageParticipant>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ChatMessageParticipant> FindById(object id)
        {
            return await _context.ChatMessageParticipants.FirstOrDefaultAsync(x => x.ChatMessageParticipantId == (int)id);
        }

        public async Task Insert(ChatMessageParticipant entity)
        {
            await _context.ChatMessageParticipants.AddAsync(entity);
        }

        public Task Update(ChatMessageParticipant entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}