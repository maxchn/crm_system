using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Чат
    /// </summary>
    public class Chat
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatId { get; set; }

        // Название
        [Required]
        public string Name { get; set; }

        // Владелец чата
        public ApplicationUser Owner { get; set; }

        // Список участников
        public List<ChatParticipant> ChatParticipants { get; set; }

        public Company Company { get; set; }

        public Chat()
        {
            ChatParticipants = new List<ChatParticipant>();
        }
    }
}