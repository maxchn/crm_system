namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Приглашение на регистрацию в системе как сотрудника
    /// </summary>
    public class EmployeeInvitation
    {
        // Идентификатор
        public int EmployeeInvitationId { get; set; }

        // Email будущего сотрудника
        public string Email { get; set; }

        // Регистрационный токен
        public string Token { get; set; }

        // Идентификатор компании в которую приглашают
        public int CompanyId { get; set; }
    }
}