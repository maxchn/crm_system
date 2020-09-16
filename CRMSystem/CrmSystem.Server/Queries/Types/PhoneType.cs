using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class PhoneType : ObjectGraphType<Phone>
    {
        public PhoneType()
        {
            Name = "Phone";

            Field(x => x.PhoneId);
            Field(x => x.PhoneNumber);
            Field(x => x.OwnerId);
        }
    }
}