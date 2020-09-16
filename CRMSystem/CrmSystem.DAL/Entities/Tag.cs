using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Тег
    /// </summary>
    public class Tag
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }

        // Название
        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Name { get; set; }

        // Список задач (many to many)
        public IList<TaskTag> TaskTags { get; set; }

        // Список компаний (many to many)
        public IList<CompanyTag> CompanyTags { get; set; }
    }
}