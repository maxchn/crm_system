using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class PositionType : ObjectGraphType<Position>
    {
        public PositionType()
        {
            Name = "Position";

            Field(x => x.PositionId).Description("Id");
            Field(x => x.Name).Description("Name");
        }
    }
}