namespace CrmSystem.DAL.Entities
{
    public class CoExecutor
    {
        public int TaskId { get; set; }
        public MTask Task { get; set; }

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
    }
}