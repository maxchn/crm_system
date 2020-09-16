using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Публичная ссылка на файл
    /// </summary>
    public class FilePublicLink
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FilePublicLinkId { get; set; }

        // Публичный код который идентифицирует файл
        public string Link { get; set; }

        // Путь к файлу
        public string Path { get; set; }

        // Сокращенная ссылка
        public ShortLink ShortLink { get; set; }

        // Создатель ссылки
        public ApplicationUser Owner { get; set; }
    }
}