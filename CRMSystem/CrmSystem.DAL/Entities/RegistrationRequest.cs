using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    public class RegistrationRequest
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegistrationRequestId { get; set; }

        // Идентификатор компании в которую приглашают сотрудника
        public int CompanyId { get; set; }

        // Регистрационный код
        public string Code { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Patronymic { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Department { get; set; }
    }
}