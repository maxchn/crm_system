using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Repositories
{
    public class NpsqlDepartmentRepository : IGenericRepository<Department>
    {
        private NpgsqlDbContext _context;

        public NpsqlDepartmentRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var department = await FindById(id);

            if (department is null)
                throw new ArgumentException("Department with the specified ID not found!!!");

            _context.Remove(department);
        }

        public async Task<IEnumerable<Department>> Find(Expression<Func<Department, bool>> predicate)
        {
            return await _context.Departments.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Department>> FindAll()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> FindById(object id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(u => u.DepartmentId == (int)id);
        }

        public async Task Insert(Department entity)
        {
            await _context.Departments.AddAsync(entity);
        }

        public Task Update(Department entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}