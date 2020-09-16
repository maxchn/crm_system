using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    public class CompanyTag
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}