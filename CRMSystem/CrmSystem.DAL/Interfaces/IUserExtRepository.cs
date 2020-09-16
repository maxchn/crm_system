using CrmSystem.DAL.Entities;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Interfaces
{
    public interface IUserExtRepository
    {
        Task UpdateAvatar(string userId, string fileName);
        Task<string> CreateRegistrationRequest(ApplicationUser employee, string senderId);
    }
}