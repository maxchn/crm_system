using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Сокращенная ссылка
    /// </summary>
    public class ShortLink
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShortLinkId { get; set; }

        // Короткая ссылка
        [Required]
        public string Short { get; set; }

        // Полная ссылка
        [Required]
        public string Full { get; set; }

        // Владелец ссылки
        public string OwnerId { get; set; }

        // Компания к которой относится вокращенная ссылка
        public int? CompanyId { get; set; }
    }
}