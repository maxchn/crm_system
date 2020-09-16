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
    public class NpsqlChatRepository : IGenericRepository<Chat>, IChatExtRepository
    {
        private NpgsqlDbContext _context;

        public NpsqlChatRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task AddNewParticipant(int chatId, IList<ChatParticipant> participants)
        {
            await _context.ChatParticipants.AddRangeAsync(participants);
        }

        public async Task Delete(object id)
        {
            var chat = await FindById(id);

            if (chat is null)
                throw new ArgumentException("Chat with the specified ID not found!!!");

            _context.Chats.Remove(chat);
        }

        public async Task<IEnumerable<Chat>> Find(Expression<Func<Chat, bool>> predicate)
        {
            return await _context.Chats
                .AsNoTracking()
                .Include(x => x.ChatParticipants)
                .Include(x => x.Company)
                .Include(x => x.Owner)
                .Include(x => x.Owner.ChatParticipants)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> FindAll()
        {
            return await _context.Chats.Include(x => x.ChatParticipants)
                .Include(x => x.ChatParticipants)
                .Include(x => x.Company)
                .Include(x => x.Owner)
                .ToListAsync();
        }

        public async Task<Chat> FindById(object id)
        {
            return await _context.Chats.Include(c => c.Owner)
                .FirstOrDefaultAsync(x => x.ChatId == (int)id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetChatParticipants(int chatId)
        {
            return await _context.ChatParticipants
                    .Include(p => p.User)
                    .Where(p => p.ChatId == chatId)
                    .Select(p => p.User)
                    .ToListAsync();
        }

        public async Task Insert(Chat entity)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyId == entity.Company.CompanyId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(entity.Owner.Id));

            entity.Company = company;
            entity.Owner = user;

            await _context.Chats.AddAsync(entity);
            await _context.SaveChangesAsync();

            var newParticipant = new ChatParticipant
            {
                User = user,
                Chat = entity
            };

            await _context.ChatParticipants.AddAsync(newParticipant);
        }

        public async Task<LeaveStatus> LeaveChat(int chatId, string userId)
        {
            var status = LeaveStatus.Leave;

            // находим чат
            var chat = await FindById(chatId);

            if (chat != null)
            {
                // находим всех участников чата
                var chatParticipants = await _context.ChatParticipants
                    .Include(c => c.User)
                    .Where(p => p.ChatId == chat.ChatId)
                    .ToListAsync();

                if (chatParticipants != null)
                {
                    var currentParticipant = chatParticipants.FirstOrDefault(p => p.Id.Equals(userId));

                    // если текущий пользователь найден и участников чата больше чем один
                    if (currentParticipant != null && chatParticipants.Count > 1)
                    {
                        // удаляем пользователя
                        _context.ChatParticipants.Remove(currentParticipant);

                        // если этот пользователь увляется владельцем чата то меняем его на нового
                        if (chat.Owner.Id.Equals(currentParticipant.Id))
                        {
                            chatParticipants.Remove(currentParticipant);
                            chat.Owner = chatParticipants[0].User;
                        }
                    }
                    else if (currentParticipant != null)
                    {
                        // если в чате только один пользователь то
                        // удаляем пользователя
                        _context.ChatParticipants.Remove(currentParticipant);

                        IEnumerable<ChatMessageParticipant> messages = await _context.ChatMessageParticipants
                            .Include(c => c.Message)
                            .Where(c => c.ChatId == chatId).ToListAsync();

                        foreach (var message in messages)
                        {
                            _context.ChatMessages.Remove(message.Message);
                        }

                        // а затем удаляем и сам чат
                        _context.Chats.Remove(chat);

                        status = LeaveStatus.ChatRemoved;
                    }
                }
            }

            return status;
        }

        public async Task RemoveParticipantFromChat(int chatId, string participantId)
        {
            var chatParticipant = await
                _context.ChatParticipants.FirstOrDefaultAsync(cp =>
                    cp.ChatId == chatId && cp.Id.Equals(participantId));

            if (chatParticipant != null)
            {
                _context.ChatParticipants.Remove(chatParticipant);
            }
        }

        public Task Update(Chat entityToUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateChatName(int chatId, string newChatName)
        {
            var chat = await FindById(chatId);
            chat.Name = newChatName;
        }
    }
}