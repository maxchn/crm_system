using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Уведомление связанное с компанией
    /// </summary>
    public class CompanyNotification
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyNotificationId { get; set; }

        // Автор возникновения события
        [Column("AuthorId")]
        public ApplicationUser Author { get; set; }

        // Идентификатор компании
        public int CompanyId { get; set; }

        // Компания к которой относится уведомление
        public Company Company { get; set; }

        // Новый сотрудник
        [Column("EmployeeId")]
        public ApplicationUser NewEmployee { get; set; }

        // Содержимое
        public string Body { get; set; }

        // Тип действия
        public CompanyNotificationType Type { get; set; }

        // Дата и время возникновения
        public DateTime DateTime { get; set; }
    }
}