using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Сообщения пользователя в чатах
    /// </summary>
    public class ChatMessageParticipant
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatMessageParticipantId { get; set; }

        public Chat Chat { get; set; }
        public int ChatId { get; set; }

        public ChatMessage Message { get; set; }
        public int MessageId { get; set; }

        public ApplicationUser User { get; set; }
        public string Id { get; set; }
    }
}