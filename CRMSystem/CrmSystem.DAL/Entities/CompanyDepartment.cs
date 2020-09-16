namespace CrmSystem.DAL.Entities
{
    public class CompanyDepartment
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}