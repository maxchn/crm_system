using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Прикрепленный файл
    /// </summary>
    public class AttachedFile
    {
        // ИД
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttachedFileId { get; set; }

        // Название файла
        public string Name { get; set; }

        // Путь
        public string Path { get; set; }

        // Владелец
        public ApplicationUser Owner { get; set; }
    }
}