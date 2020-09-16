using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Событие в календаре
    /// </summary>
    public class CalendarEvent
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalendarEventId { get; set; }

        // Название события
        public string Text { get; set; }

        // Начало события
        public DateTime Start { get; set; }

        // Конец события
        public DateTime End { get; set; }

        // Владелец
        public ApplicationUser Author { get; set; }

        public string AuthorId { get; set; }
    }
}