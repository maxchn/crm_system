using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class GenderType : ObjectGraphType<Gender>
    {
        public GenderType()
        {
            Name = "Gender";

            Field(x => x.GenderId).Description("GenderId");
            Field(x => x.Name).Description("Name");
        }
    }
}