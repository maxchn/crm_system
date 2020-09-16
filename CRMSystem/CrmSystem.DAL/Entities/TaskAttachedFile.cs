using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Прикрепленый файл к задаче
    /// </summary>
    public class TaskAttachedFile
    {
        // ИД
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskAttachedFileId { get; set; }

        // Идентификатор файла
        public int AttachedFileId { get; set; }

        // Файл
        public AttachedFile AttachedFile { get; set; }

        // Идетификатор задачи
        public int TaskId { get; set; }

        // Задача
        public MTask Task { get; set; }
    }
}