using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class AuthorInputType : InputObjectGraphType
    {
        public AuthorInputType()
        {
            Name = "AuthorInput";

            Field<NonNullGraphType<StringGraphType>>("Id");
        }
    }
}