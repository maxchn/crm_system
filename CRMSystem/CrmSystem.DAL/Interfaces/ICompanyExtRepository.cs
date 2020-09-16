using CrmSystem.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Interfaces
{
    public interface ICompanyExtRepository<T>
    {
        Task AddEmployee(int id, T entity);
        Task<IList<ApplicationUser>> GetAllEmployees(int id);

        Task<IList<Company>> FindByEmployee(string id);
    }
}