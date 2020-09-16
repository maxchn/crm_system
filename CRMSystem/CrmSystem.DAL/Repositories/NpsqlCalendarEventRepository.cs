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
    public class NpsqlCalendarEventRepository : IGenericRepository<CalendarEvent>
    {
        private NpgsqlDbContext _context;

        public NpsqlCalendarEventRepository(NpgsqlDbContext context)
        {
            _context = context;
        }

        public async Task Delete(object id)
        {
            var calendarEvent = await FindById(id);

            if (calendarEvent is null)
                throw new ArgumentException("Calendar Event with the specified ID not found!!!");

            _context.CalendarEvents.Remove(calendarEvent);
        }

        public async Task<IEnumerable<CalendarEvent>> Find(Expression<Func<CalendarEvent, bool>> predicate)
        {
            return await _context.CalendarEvents
                .Include(c => c.Author)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CalendarEvent>> FindAll()
        {
            return await _context.CalendarEvents
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<CalendarEvent> FindById(object id)
        {
            return await _context.CalendarEvents.Include(c => c.Author)
                .FirstOrDefaultAsync(x => x.CalendarEventId == (int)id);
        }

        public async Task Insert(CalendarEvent entity)
        {
            await _context.CalendarEvents.AddAsync(entity);
        }

        public async Task Update(CalendarEvent entityToUpdate)
        {
            var calendarEvent = await FindById(entityToUpdate.CalendarEventId);

            if (calendarEvent is null)
                throw new ArgumentException("Calendar Event with the specified ID not found!!!");

            calendarEvent.Text = entityToUpdate.Text;
            calendarEvent.Start = entityToUpdate.Start;
            calendarEvent.End = entityToUpdate.End;
        }
    }
}