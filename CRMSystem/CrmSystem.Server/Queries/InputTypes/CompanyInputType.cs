using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class CompanyInputType : InputObjectGraphType
    {
        public CompanyInputType()
        {
            Name = "CompanyInput";

            Field<NonNullGraphType<IntGraphType>>("CompanyId");
            Field<NonNullGraphType<StringGraphType>>("Name");
            Field<NonNullGraphType<StringGraphType>>("UrlName");
        }
    }
}
