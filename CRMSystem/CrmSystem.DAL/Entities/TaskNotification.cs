using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Уведомление связанное с задачей
    /// </summary>
    public class TaskNotification
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskNotificationId { get; set; }

        // Автор возникновения события
        [Column("AuthorId")]
        public ApplicationUser Author { get; set; }

        // Задача с которой связанно уведомление
        public MTask Task { get; set; }

        // Тип действия
        public TaskNotificationType Type { get; set; }

        // Дата и время возникновения
        public DateTime DateTime { get; set; }
    }
}