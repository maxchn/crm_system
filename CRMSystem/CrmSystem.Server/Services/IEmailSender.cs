using System.Threading.Tasks;

namespace CrmSystem.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}