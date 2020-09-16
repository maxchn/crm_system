using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Компания
    /// </summary>
    public class Company
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }

        // Название
        public string Name { get; set; }

        // URL адрес компании
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Адрес компании может состоять только латинских символов, цифр, символов тире и нижнее подчеркивания")]
        public string UrlName { get; set; }

        // Владелец
        public ApplicationUser Owner { get; set; }

        // Сотрудники
        public IList<CompanyEmployee> CompanyEmployees { get; set; }

        // Отделы (many to many)
        public IList<CompanyDepartment> CompanyDepartments { get; set; }

        // Должности (many to many)
        public IList<CompanyPosition> CompanyPositions { get; set; }

        // Задачи
        public IList<MTask> Tasks { get; set; }

        // Список тегов (many to many)
        public IList<CompanyTag> CompanyTags { get; set; }

        public Company()
        {
            CompanyEmployees = new List<CompanyEmployee>();
            CompanyDepartments = new List<CompanyDepartment>();
            CompanyPositions = new List<CompanyPosition>();
            Tasks = new List<MTask>();
            CompanyTags = new List<CompanyTag>();
        }
    }
}