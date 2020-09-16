using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CrmSystem.Server.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            string login = _configuration["Email:Login"];
            string password = _configuration["Email:Password"];
            int.TryParse(_configuration["Email:Port"], out int port);

            SmtpClient client = new SmtpClient(_configuration["Email:Host"], port);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(login, password);

            MailMessage mail = new MailMessage(login, email);
            mail.Body = message;
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);
        }
    }
}
