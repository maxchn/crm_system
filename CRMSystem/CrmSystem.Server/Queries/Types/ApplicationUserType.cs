using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class ApplicationUserType : ObjectGraphType<ApplicationUser>
    {
        public ApplicationUserType()
        {
            Name = "ApplicationUser";

            Field(x => x.Id);
            Field(x => x.UserName);
            Field(x => x.Email);
            Field(x => x.DateOfBirth);
            Field<GenderType>("Gender");
            Field(x => x.LastName);
            Field(x => x.FirstName);
            Field(x => x.Patronymic);
            Field<PositionType>("Position");
            Field<DepartmentType>("Department");
            Field<ListGraphType<PhoneType>>("Phones");
            Field(x => x.IsFullProfile);
        }
    }
}