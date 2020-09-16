using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    public class TaskTag
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TaskId { get; set; }
        public MTask Task { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}