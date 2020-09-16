using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Персональное уведомление
    /// </summary>
    public class PrivateNotification
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrivateNotificationId { get; set; }

        // Идентификатор автора
        public string AuthorId { get; set; }

        // Автор
        public ApplicationUser Author { get; set; }

        // Тело уведомления
        public string Body { get; set; }

        // Дата и время возникновения
        public DateTime DateTime { get; set; }
    }
}