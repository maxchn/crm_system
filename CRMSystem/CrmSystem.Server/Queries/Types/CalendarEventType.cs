using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class CalendarEventType : ObjectGraphType<CalendarEvent>
    {
        public CalendarEventType()
        {
            Name = "CalendarEvent";

            Field(x => x.CalendarEventId);
            Field(x => x.Text);
            Field<DateTimeGraphType>("Start");
            Field<DateTimeGraphType>("End");
            Field(x => x.AuthorId);
            Field<ApplicationUserType>("Author");
        }
    }
}