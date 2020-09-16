using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Сообщение чата
    /// </summary>
    public class ChatMessage
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatMessageId { get; set; }

        // Текст сообщения
        public string Text { get; set; }

        // Дата и время отправки
        public DateTime DispatchTime { get; set; }

        // Отправитель
        public ApplicationUser Owner { get; set; }

        public string OwnerId { get; set; }
    }
}