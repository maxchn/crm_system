using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Пол
    /// </summary>
    public class Gender
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderId { get; set; }

        // Название
        [Required]
        public string Name { get; set; }

        // Список пользователей
        [JsonIgnore]
        public IList<ApplicationUser> Users { get; set; }
    }
}