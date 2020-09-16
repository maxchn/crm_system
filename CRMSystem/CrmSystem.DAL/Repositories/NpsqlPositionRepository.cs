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
    public class NpsqlPositionRepository : IGenericRepository<Position>
    {
        private NpgsqlDbContext _context;

        public NpsqlPositionRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var position = await FindById(id);

            if (position is null)
                throw new ArgumentException("Position with the specified ID not found!!!");

            _context.Positions.Remove(position);
        }

        public async Task<IEnumerable<Position>> Find(Expression<Func<Position, bool>> predicate)
        {
            return await _context.Positions.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Position>> FindAll()
        {
            return await _context.Positions.ToListAsync();
        }

        public async Task<Position> FindById(object id)
        {
            return await _context.Positions
                .FirstOrDefaultAsync(u => u.PositionId == (int)id);
        }

        public async Task Insert(Position entity)
        {
            await _context.Positions.AddAsync(entity);
        }

        public Task Update(Position entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}