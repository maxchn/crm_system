using CrmSystem.DAL.Entities;

namespace CrmSystem.Server.GraphQL
{
    public class EmployeeRegisterResult
    {
        public bool Status { get; set; }

        public ApplicationUser Employee { get; set; }
    }
}