using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Номер телефона
    /// </summary>
    public class Phone
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhoneId { get; set; }

        // Номер телефона
        public string PhoneNumber { get; set; }

        // Идентификатор владельца номера телефона
        public string OwnerId { get; set; }

        // Владелец номера телефона
        [JsonIgnore]
        public ApplicationUser Owner { get; set; }
    }
}