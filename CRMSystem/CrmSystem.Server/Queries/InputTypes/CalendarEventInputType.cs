using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class CalendarEventInputType : InputObjectGraphType
    {
        public CalendarEventInputType()
        {
            Name = "CalendarEventInput";

            Field<IntGraphType>("CalendarEventId");
            Field<StringGraphType>("Text");
            Field<DateTimeGraphType>("Start");
            Field<DateTimeGraphType>("End");
            Field<StringGraphType>("AuthorId");
        }
    }
}