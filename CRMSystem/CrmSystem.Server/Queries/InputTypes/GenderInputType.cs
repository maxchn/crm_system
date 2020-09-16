using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class GenderInputType : InputObjectGraphType
    {
        public GenderInputType()
        {
            Name = "GenderInputType";

            Field<NonNullGraphType<IntGraphType>>("GenderId");
            Field<StringGraphType>("Name");
        }
    }
}