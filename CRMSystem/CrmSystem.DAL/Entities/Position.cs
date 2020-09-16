using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Должность
    /// </summary>
    public class Position
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PositionId { get; set; }

        // Название
        [Required]
        public string Name { get; set; }

        // Список пользователей
        [JsonIgnore]
        public IList<ApplicationUser> Users { get; set; }

        // Список компаний
        [JsonIgnore]
        public IList<CompanyPosition> CompanyPositions { get; set; }
    }
}