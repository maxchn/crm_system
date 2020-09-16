using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TagInputType : InputObjectGraphType
    {
        public TagInputType()
        {
            Name = "TagInput";

            Field<IntGraphType>("TagId");
            Field<NonNullGraphType<StringGraphType>>("Name");
        }
    }
}