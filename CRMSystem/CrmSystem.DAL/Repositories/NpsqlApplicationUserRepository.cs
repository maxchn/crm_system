using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CrmSystem.DAL.Repositories
{
    public class NpsqlApplicationUserRepository : IGenericRepository<ApplicationUser>, IUserExtRepository
    {
        private NpgsqlDbContext _context;

        public NpsqlApplicationUserRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateRegistrationRequest(ApplicationUser employee, string senderId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(senderId));
                var company = await _context.Companies
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.Owner.Id.Equals(user.Id));

                if (user is null)
                    throw new Exception("User not found!!!");

                if (company is null)
                    throw new Exception("Company not found!!!");

                RegistrationRequest newRequest = new RegistrationRequest
                {
                    Email = employee.Email,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Patronymic = employee.Patronymic,
                    Position = employee.Position.Name,
                    Department = employee.Department.Name,
                    Code = $"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}",
                    CompanyId = company.CompanyId
                };

                await _context.RegistrationRequests.AddAsync(newRequest);
                await _context.SaveChangesAsync();
                return newRequest.Code;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public Task Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> Find(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _context.Users
                .Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> FindAll()
        {
            return await _context.Users.Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Department)
                .Include(u => u.Gender)
                .Include(u => u.Phones)
                .ToListAsync();
        }

        public async Task<ApplicationUser> FindById(object id)
        {
            return await _context.Users.Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Gender)
                .Include(u => u.ChatParticipants)
                .Include(u => u.CompanyEmployees)
                .Include(u => u.Phones)
                .FirstOrDefaultAsync(u => u.Id.Equals(id.ToString()));
        }

        public Task Insert(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public async Task Update(ApplicationUser entityToUpdate)
        {
            var user = await FindById(entityToUpdate.Id);

            if (user is null)
                throw new ArgumentException("User with the specified ID not found!!!");

            user.LastName = entityToUpdate.LastName;
            user.FirstName = entityToUpdate.FirstName;
            user.Patronymic = entityToUpdate.Patronymic;
            user.DateOfBirth = entityToUpdate.DateOfBirth;

            var gender = await _context.Genders.FirstOrDefaultAsync(g => g.GenderId == entityToUpdate.Gender.GenderId);
            user.Gender = gender;

            var department = await _context.Departments.FirstOrDefaultAsync(d =>
                d.Name.ToLower().Equals(entityToUpdate.Department.Name.ToLower()));

            if (department is null)
            {
                await _context.Departments.AddAsync(entityToUpdate.Department);
                await _context.SaveChangesAsync();
                department = entityToUpdate.Department;
            }

            user.Department = department;

            var position = await _context.Positions.FirstOrDefaultAsync(p =>
                p.Name.ToLower().Equals(entityToUpdate.Position.Name.ToLower()));

            if (position is null)
            {
                await _context.Positions.AddAsync(entityToUpdate.Position);
                await _context.SaveChangesAsync();
                position = entityToUpdate.Position;
            }

            _context.Phones.RemoveRange(user.Phones);

            user.Phones?.Clear();
            user.Phones = entityToUpdate.Phones;

            user.Position = position;
            user.IsFullProfile = true;
        }

        public async Task UpdateAvatar(string userId, string fileName)
        {
            var user = await FindById(userId);

            if (user is null)
                throw new ArgumentException("User with the specified ID not found!!!");

            user.AvatarPath = fileName;
        }
    }
}