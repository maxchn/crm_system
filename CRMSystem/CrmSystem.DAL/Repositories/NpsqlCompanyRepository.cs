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
    public class NpsqlCompanyRepository : IGenericRepository<Company>, ICompanyExtRepository<ApplicationUser>
    {
        private NpgsqlDbContext _context;

        public NpsqlCompanyRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task AddEmployee(int id, ApplicationUser entity)
        {
            var company = await FindById(id);

            if (company is null)
                throw new ArgumentException("Company with the specified ID not found!!!");

            var companyEmployee = new CompanyEmployee();
            companyEmployee.Id = entity.Id;
            companyEmployee.CompanyId = id;

            await _context.CompanyEmployees.AddAsync(companyEmployee);
        }

        public Task Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Company>> Find(Expression<Func<Company, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Company>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Company>> FindByEmployee(string id)
        {
            return await _context.CompanyEmployees
                .Include(c => c.Company)
                .Where(c => c.Id.Equals(id))
                .Select(c => c.Company)
                .ToListAsync();
        }

        public async Task<Company> FindById(object id)
        {
            return await _context.Companies.Include(c => c.Owner)
                .FirstOrDefaultAsync(u => u.CompanyId == (int)id);
        }

        public async Task<IList<ApplicationUser>> GetAllEmployees(int id)
        {
            var tmp = await _context.CompanyEmployees
                .Include(t => t.User)
                .Include(t => t.User.Position)
                .Include(t => t.User.Department)
                .Where(t => t.CompanyId == id)
                //.Select(t => t.User)
                .ToListAsync();

            return tmp.Select(t => t.User).ToList();
        }

        public async Task Insert(Company entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task Update(Company entityToUpdate)
        {
            var company = await FindById(entityToUpdate.CompanyId);

            if (company is null)
                throw new ArgumentException("Company with the specified ID not found!!!");

            company.Name = entityToUpdate.Name;
            company.UrlName = entityToUpdate.UrlName;
        }
    }
}