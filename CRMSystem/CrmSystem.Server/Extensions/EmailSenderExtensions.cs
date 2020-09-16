using CrmSystem.Server.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CrmSystem.Server.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Пожалуйста подтвердите Ваш аккаунт нажав на <a href='{HtmlEncoder.Default.Encode(link)}'>эту ссылку</a>.");
        }

        public static Task SendInvitationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "MCRMSystem: Приглашение в CRMSystem",
                $@"<p>Приглашаю вас в CRM систему нашей компании. Здесь мы сможем вместе работать над проектами и задачами, управлять документами, планировать встречи и собрания, общаться в чатах и многое другое.</p>
                   <p><a href='{HtmlEncoder.Default.Encode(link)}'>Перейти на страницу регистрации</a></p>
                   <p>Для авторизации используйте ваш e-mail как логин. При первой авторизации на корпоративном портале пароль вам нужно будет придумать.</p>");
        }

        public static Task SendRegestrationDateAsync(this IEmailSender emailSender, string sender, string email, string password, string link)
        {
            return emailSender.SendEmailAsync(email, "Приглашение в MCRMSystem",
                $@"<p>{sender} приглашет вас в CRM систему нашей компании. Здесь мы сможем вместе работать над проектами и задачами, управлять документами, планировать встречи и собрания, общаться в чатах и многое другое.</p>
                   <br/>
                   <p>Вы были зарегистрированны в системе уполномоченым лицом компании.</p>
                   <br/>
                   <p>Ваши учетные данные от личного кабинета:</p>
                   <p>Логин: <strong><a href=\""mailto:{email}\"">{email}</a></strong></p>
                   <p>Пароль: <strong>{password}</strong></p>
                   <br/>
                   <p>Для входа в личный кабинет нажмите <a href='{HtmlEncoder.Default.Encode(link)}'>здесь</a>.</p>");
        }
    }
}