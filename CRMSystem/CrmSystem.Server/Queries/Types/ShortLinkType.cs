using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class ShortLinkType : ObjectGraphType<ShortLink>
    {
        public ShortLinkType()
        {
            Name = "ShortLink";

            Field(x => x.ShortLinkId);
            Field(x => x.Full);
            Field(x => x.Short);
            Field(x => x.OwnerId);
            Field<IntGraphType>("CompanyId");
        }
    }
}