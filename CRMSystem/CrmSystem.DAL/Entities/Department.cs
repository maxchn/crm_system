using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Отдел
    /// </summary>
    public class Department
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        // Название
        [Required]
        public string Name { get; set; }

        // Список сотрудников (many to many)
        [JsonIgnore]
        public IList<ApplicationUser> Users { get; set; }

        // Список компаний (many to many)
        [JsonIgnore]
        public IList<CompanyDepartment> CompanyDepartments { get; set; }
    }
}