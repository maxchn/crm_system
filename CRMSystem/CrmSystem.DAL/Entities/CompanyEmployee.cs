using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    public class CompanyEmployee
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyEmployeeId { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
    }
}