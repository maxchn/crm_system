using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TagType : ObjectGraphType<Tag>
    {
        public TagType()
        {
            Name = "Tag";

            Field(x => x.TagId);
            Field(x => x.Name);
        }
    }
}