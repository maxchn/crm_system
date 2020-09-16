using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class PositionInputType : InputObjectGraphType
    {
        public PositionInputType()
        {
            Name = "PositionInputType";

            Field<IntGraphType>("PositionId");
            Field<NonNullGraphType<StringGraphType>>("Name");
        }
    }
}