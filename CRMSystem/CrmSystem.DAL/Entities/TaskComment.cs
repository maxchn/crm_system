using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Комментарий к задаче
    /// </summary>
    public class TaskComment
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskCommentId { get; set; }

        // Задача к которой относится коментарий
        public MTask Task { get; set; }

        // Автор
        public ApplicationUser Author { get; set; }

        // Идентификатор автора
        public string AuthorId { get; set; }

        // Сообщение
        public string Text { get; set; }

        // Дата
        public DateTime Date { get; set; }

        // Может ли текущий пользователь удалять текущий коментарий
        [NotMapped]
        public bool IsAccessOnDeleting { get; set; }
    }
}