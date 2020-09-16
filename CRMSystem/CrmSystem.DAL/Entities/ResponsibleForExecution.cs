namespace CrmSystem.DAL.Entities
{
    public class ResponsibleForExecution
    {
        public int TaskId { get; set; }
        public MTask Task { get; set; }

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
    }
}