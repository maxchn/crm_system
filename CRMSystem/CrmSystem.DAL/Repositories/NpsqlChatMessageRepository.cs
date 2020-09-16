using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Repositories
{
    public class NpsqlChatMessageRepository : IGenericRepository<ChatMessage>
    {
        private NpgsqlDbContext _context;

        public NpsqlChatMessageRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var message = await FindById(id);

            if (message is null)
                throw new ArgumentException("ChatMessage with the specified ID not found!!!");

            _context.ChatMessages.Remove(message);
        }

        public Task<IEnumerable<ChatMessage>> Find(Expression<Func<ChatMessage, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ChatMessage>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ChatMessage> FindById(object id)
        {
            return await _context.ChatMessages.FirstOrDefaultAsync(x => x.ChatMessageId == (int)id);
        }

        public async Task Insert(ChatMessage entity)
        {
            await _context.ChatMessages.AddAsync(entity);
        }

        public Task Update(ChatMessage entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}