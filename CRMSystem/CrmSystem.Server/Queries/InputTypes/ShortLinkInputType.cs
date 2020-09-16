using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class ShortLinkInputType : InputObjectGraphType
    {
        public ShortLinkInputType()
        {
            Name = "ShortLinkInput";

            Field<IntGraphType>("ShortLinkId");
            Field<StringGraphType>("Full");
            Field<StringGraphType>("Short");
            Field<StringGraphType>("OwnerId");
            Field<IntGraphType>("CompanyId");
        }
    }
}