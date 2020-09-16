namespace CrmSystem.DAL.Entities
{
    public class CompanyPosition
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }
    }
}