using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmSystem.DAL.Entities
{
    /// <summary>
    /// Пользователя (сотрудник)
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Имя
        public string FirstName { get; set; }

        // Фамилия
        public string LastName { get; set; }

        // Отчество
        public string Patronymic { get; set; }

        // Пол
        public Gender Gender { get; set; }

        // Дата рождения
        public DateTime DateOfBirth { get; set; }

        // Путь к аватару
        public string AvatarPath { get; set; }

        // Статус
        public bool IsOnline { get; set; }

        // Полностью заполнен профиль?
        public bool IsFullProfile { get; set; } = false;

        // Должность
        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }

        // Отдел
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        // Соисполнители (many to many)
        [JsonIgnore]
        public IList<CoExecutor> CoExecutors { get; set; }

        // Наблюдатели(many to many)
        [JsonIgnore]
        public IList<Observer> Observers { get; set; }

        // Ответственные за выполнение(many to many)
        [JsonIgnore]
        public IList<ResponsibleForExecution> ResponsiblesForExecution { get; set; }

        // Список компаний
        [JsonIgnore]
        public IList<CompanyEmployee> CompanyEmployees { get; set; }

        // Список чатов
        [JsonIgnore]
        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }

        // Список контактных телефонов
        public IList<Phone> Phones { get; set; }
    }
}