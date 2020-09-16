using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class PhoneInputType : InputObjectGraphType
    {
        public PhoneInputType()
        {
            Name = "PhoneInput";

            Field<IntGraphType>("PhoneId");
            Field<NonNullGraphType<StringGraphType>>("PhoneNumber");
            Field<NonNullGraphType<StringGraphType>>("OwnerId");
        }
    }
}