using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    public class PrivateNotificationEmployee
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PrivateNotificationId { get; set; }
        public PrivateNotification PrivateNotification { get; set; }

        public string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}