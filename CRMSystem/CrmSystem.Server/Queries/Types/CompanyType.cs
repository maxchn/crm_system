using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class CompanyType : ObjectGraphType<Company>
    {
        public CompanyType()
        {
            Name = "Company";

            Field(x => x.CompanyId);
            Field(x => x.Name);
            Field(x => x.UrlName);
            Field<ApplicationUserType>("Owner");
        }
    }
}