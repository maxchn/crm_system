using CrmSystem.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Interfaces
{
    public interface IChatExtRepository
    {
        Task AddNewParticipant(int chatId, IList<ChatParticipant> participants);
        Task UpdateChatName(int chatId, string newChatName);
        Task<IEnumerable<ApplicationUser>> GetChatParticipants(int chatId);
        Task RemoveParticipantFromChat(int chatId, string participantId);
        Task<LeaveStatus> LeaveChat(int chatId, string userId);
        //Task<Chat> FindOne()
    }
}
