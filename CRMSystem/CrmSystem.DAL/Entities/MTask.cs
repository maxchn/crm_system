using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Задача
    /// </summary>
    public class MTask
    {
        // Идентификатор
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        // Название
        [Required]
        public string Name { get; set; }

        // Подробное описание
        [Required]
        public string Body { get; set; }

        // Дата создания
        public DateTime CreatedDate { get; set; }

        // Конечный срок
        [Required]
        public DateTime Deadline { get; set; }

        // Автор
        public ApplicationUser Author { get; set; }

        // Сейчас идет выполнение?
        public bool IsExecution { get; set; }

        // Начало выполнения
        public DateTime StartExecution { get; set; }

        // Конец выполнения
        public DateTime EndExecution { get; set; }

        // Конечный исполнитель
        public string FinalPerformerId { get; set; }

        [NotMapped]
        public ApplicationUser FinalPerformer { get; set; }

        // Общее время затраченное на выполнение текущей задачи
        public double TotalTime { get; set; }

        // Это важная задача
        public bool IsImportant { get; set; }

        // Компания к которой относится задача 
        public Company Company { get; set; }

        // Соисполнители (many to many)
        public IList<CoExecutor> CoExecutors { get; set; }

        // Наблюдатели (many to many)
        public IList<Observer> Observers { get; set; }

        // Ответственные за выполнение (many to many)
        public IList<ResponsibleForExecution> ResponsiblesForExecution { get; set; }

        // Теги (many to many)
        public IList<TaskTag> TaskTags { get; set; }

        // Прикрепленные файлы
        public List<TaskAttachedFile> AttachedFiles { get; set; }

        public MTask()
        {
            CoExecutors = new List<CoExecutor>();
            Observers = new List<Observer>();
            ResponsiblesForExecution = new List<ResponsibleForExecution>();
            TaskTags = new List<TaskTag>();
            AttachedFiles = new List<TaskAttachedFile>();
        }
    }
}